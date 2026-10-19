using Avalonia.Threading;
using OmniEMU.Ava;
using OmniEMU.Common.Configuration;
using OmniEMU.Common.Logging;
using OmniEMU.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;

namespace OmniEMU.Modules
{
    /// <summary>
    /// Queue-based download engine for the in-app homebrew catalog and direct URL
    /// downloads. Files are streamed into the OmniEMU downloads folder, optionally
    /// verified against the SHA-256 digest published by GitHub, and safe-extracted
    /// when they are zip archives. This manager never downloads Nintendo
    /// copyrighted content; catalog entries are official homebrew projects only.
    /// </summary>
    internal static class DownloadManager
    {
        private enum DownloadOutcome
        {
            Completed,
            Failed,
            NotFound,
        }

        private static readonly string[] _preferredAssetExtensions = { ".nro", ".ovl", ".zip", ".7z", ".tar.gz", ".nsp", ".nca" };

        private static readonly HashSet<string> _supportedDirectExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".nro", ".nsp", ".xci", ".nca", ".nso", ".zip", ".7z", ".rar", ".tar", ".gz", ".tgz", ".jar", ".ovl",
        };

        private static readonly HttpClient _http;
        private static readonly DownloadsJsonSerializerContext _serializerContext = new(JsonHelper.GetDefaultSerializerOptions());

        /// <summary>Download queue, mutated on the UI thread only.</summary>
        public static ObservableCollection<DownloadItem> Items { get; } = new();

        /// <summary>Raised on the UI thread whenever an item finishes successfully.</summary>
        public static event Action<DownloadItem> ItemCompleted;

        static DownloadManager()
        {
            _http = new HttpClient
            {
                Timeout = Timeout.InfiniteTimeSpan,
            };

            _http.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", BuildUserAgent());
        }

        public static bool IsSupportedDirectExtension(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            return _supportedDirectExtensions.Contains(Path.GetExtension(fileName));
        }

        public static string GetSupportedExtensionList()
        {
            return string.Join(", ", _supportedDirectExtensions);
        }

        /// <summary>
        /// Queue a direct download from a URL the user provided. The file name is
        /// sanitized and must use a supported software/game extension.
        /// </summary>
        public static DownloadItem EnqueueDirect(string url, string fileName)
        {
            if (!Uri.TryCreate(url?.Trim(), UriKind.Absolute, out Uri uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("Enter a valid http:// or https:// URL to download from.");
            }

            string safeName = SanitizeFileName(fileName);

            if (string.IsNullOrWhiteSpace(safeName))
            {
                throw new ArgumentException("Enter a file name for the download, for example app.nro.");
            }

            if (!IsSupportedDirectExtension(safeName))
            {
                throw new ArgumentException(
                    $"Unsupported file type \"{Path.GetExtension(safeName)}\". Supported types: {GetSupportedExtensionList()}.");
            }

            DownloadItem item = new()
            {
                Title = safeName,
                Source = uri.Host,
                Url = uri.AbsoluteUri,
                DirectUrl = uri.AbsoluteUri,
                DirectFileName = safeName,
                Cancellation = new CancellationTokenSource(),
            };

            Items.Add(item);
            _ = Task.Run(() => RunDirectAsync(item));

            return item;
        }

        /// <summary>Queue a download from the curated homebrew catalog.</summary>
        public static DownloadItem EnqueueCatalog(CatalogEntry entry)
        {
            if (entry == null)
            {
                throw new ArgumentException("Select a catalog entry to download.");
            }

            DownloadItem existing = Items.FirstOrDefault(item =>
                ReferenceEquals(item.CatalogSource, entry) && item.IsRunning);

            if (existing != null)
            {
                return existing;
            }

            DownloadItem item = new()
            {
                Title = entry.Title,
                Source = $"{entry.Author} · {entry.Category}",
                Url = entry.PageUrl,
                CatalogSource = entry,
                Cancellation = new CancellationTokenSource(),
            };

            Items.Add(item);
            _ = Task.Run(() => RunCatalogAsync(item, entry));

            return item;
        }

        public static void Cancel(DownloadItem item)
        {
            if (item == null || !item.IsRunning)
            {
                return;
            }

            item.Cancellation?.Cancel();
        }

        public static void Retry(DownloadItem item)
        {
            // Only failed or canceled items can be retried.
            if (item == null || item.Status is not (DownloadStatus.Failed or DownloadStatus.Canceled))
            {
                return;
            }

            item.Cancellation = new CancellationTokenSource();

            Dispatcher.UIThread.Post(() =>
            {
                item.ErrorMessage = string.Empty;
                item.Progress = 0;
                item.IsProgressIndeterminate = false;
                item.StatusText = "Queued";
                item.Status = DownloadStatus.Queued;
            });

            if (item.CatalogSource != null)
            {
                _ = Task.Run(() => RunCatalogAsync(item, item.CatalogSource));
            }
            else
            {
                _ = Task.Run(() => RunDirectAsync(item));
            }
        }

        public static void ClearFinished()
        {
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                DownloadStatus status = Items[i].Status;

                if (status is DownloadStatus.Completed or DownloadStatus.Failed or DownloadStatus.Canceled)
                {
                    Items.RemoveAt(i);
                }
            }
        }

        public static string GetDownloadsDirectory()
        {
            string directory = AppDataManager.DownloadsDirPath;

            if (string.IsNullOrEmpty(directory))
            {
                directory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    "OmniEMU",
                    "downloads");
            }

            Directory.CreateDirectory(directory);

            return directory;
        }

        private static async Task RunDirectAsync(DownloadItem item)
        {
            try
            {
                PostState(item, DownloadStatus.Resolving, "Connecting…", 0, true);

                CancellationToken token = item.Cancellation?.Token ?? CancellationToken.None;
                token.ThrowIfCancellationRequested();

                await TryDownloadAsync(item, item.DirectUrl, item.DirectFileName, null, failOnNotFound: true, token);
            }
            catch (OperationCanceledException)
            {
                MarkCanceled(item);
            }
            catch (Exception ex)
            {
                SetFailed(item, FriendlyError(ex));
            }
        }

        private static async Task RunCatalogAsync(DownloadItem item, CatalogEntry entry)
        {
            try
            {
                CancellationToken token = item.Cancellation?.Token ?? CancellationToken.None;
                token.ThrowIfCancellationRequested();

                // Fast path: a stable asset name on the latest GitHub release.
                if (!string.IsNullOrEmpty(entry.Repo) && !string.IsNullOrEmpty(entry.AssetName))
                {
                    string pinnedUrl = $"https://github.com/{entry.Repo}/releases/latest/download/{entry.AssetName}";

                    DownloadOutcome outcome = await TryDownloadAsync(
                        item, pinnedUrl, entry.AssetName, null, failOnNotFound: false, token);

                    if (outcome != DownloadOutcome.NotFound)
                    {
                        return;
                    }
                }

                if (string.IsNullOrEmpty(entry.Repo))
                {
                    SetFailed(item, "This catalog entry has no downloadable source.");
                    return;
                }

                PostState(item, DownloadStatus.Resolving, "Looking up the latest release…", null, true);

                (string url, string fileName, string sha256) = await ResolveLatestAssetAsync(entry, token);

                token.ThrowIfCancellationRequested();

                await TryDownloadAsync(item, url, fileName, sha256, failOnNotFound: true, token);
            }
            catch (OperationCanceledException)
            {
                MarkCanceled(item);
            }
            catch (Exception ex)
            {
                SetFailed(item, FriendlyError(ex));
            }
        }

        private static async Task<(string Url, string FileName, string Sha256)> ResolveLatestAssetAsync(
            CatalogEntry entry, CancellationToken token)
        {
            string requestUrl = $"https://api.github.com/repos/{entry.Repo}/releases/latest";

            using HttpRequestMessage request = new(HttpMethod.Get, requestUrl);
            request.Headers.TryAddWithoutValidation("User-Agent", BuildUserAgent());
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
            request.Headers.TryAddWithoutValidation("X-GitHub-Api-Version", "2022-11-28");

            using HttpResponseMessage response = await _http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token);

            if (response.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.TooManyRequests)
            {
                throw new HttpRequestException(
                    "GitHub is rate-limiting requests from this network. Wait a few minutes and try again.");
            }

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new HttpRequestException($"No published release was found for \"{entry.Repo}\".");
            }

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync(token);
            GithubReleaseResponse release = JsonHelper.Deserialize(json, _serializerContext.GithubReleaseResponse);

            GithubReleaseAssetResponse asset = PickAsset(release?.Assets, entry.PreferredAssetExtension);

            if (asset == null || string.IsNullOrEmpty(asset.BrowserDownloadUrl))
            {
                throw new HttpRequestException(
                    $"The latest release of \"{entry.Title}\" has no downloadable asset with a supported type.");
            }

            string sha256 = null;

            if (!string.IsNullOrEmpty(asset.Digest) &&
                asset.Digest.StartsWith("sha256:", StringComparison.OrdinalIgnoreCase))
            {
                sha256 = asset.Digest.Substring("sha256:".Length);
            }

            return (asset.BrowserDownloadUrl, asset.Name, sha256);
        }

        private static GithubReleaseAssetResponse PickAsset(
            List<GithubReleaseAssetResponse> assets, string preferredExtension)
        {
            if (assets == null || assets.Count == 0)
            {
                return null;
            }

            List<GithubReleaseAssetResponse> usable = assets
                .Where(asset => asset?.Name != null && (asset.State == null || asset.State == "uploaded"))
                .ToList();

            if (!string.IsNullOrEmpty(preferredExtension))
            {
                GithubReleaseAssetResponse preferred = usable.FirstOrDefault(asset =>
                    asset.Name.EndsWith(preferredExtension, StringComparison.OrdinalIgnoreCase));

                if (preferred != null)
                {
                    return preferred;
                }
            }

            foreach (string extension in _preferredAssetExtensions)
            {
                GithubReleaseAssetResponse match = usable.FirstOrDefault(asset =>
                    asset.Name.EndsWith(extension, StringComparison.OrdinalIgnoreCase));

                if (match != null)
                {
                    return match;
                }
            }

            return usable.FirstOrDefault(asset => IsSupportedDirectExtension(asset.Name));
        }

        private static async Task<DownloadOutcome> TryDownloadAsync(
            DownloadItem item,
            string url,
            string fileName,
            string expectedSha256,
            bool failOnNotFound,
            CancellationToken token)
        {
            string tempPath = null;

            try
            {
                token.ThrowIfCancellationRequested();

                PostState(item, DownloadStatus.Resolving, "Connecting…", null, true);

                using HttpResponseMessage response = await _http.GetAsync(
                    url, HttpCompletionOption.ResponseHeadersRead, token);

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    if (failOnNotFound)
                    {
                        SetFailed(item, "The download was not found (HTTP 404). Check the URL or project releases and try again.");
                        return DownloadOutcome.Failed;
                    }

                    return DownloadOutcome.NotFound;
                }

                response.EnsureSuccessStatusCode();

                string directory = GetDownloadsDirectory();
                string finalPath = GetUniquePath(directory, fileName);
                tempPath = finalPath + ".part";

                long total = response.Content.Headers.ContentLength ?? -1;
                long received = 0;
                long lastProgressPost = 0;

                Dispatcher.UIThread.Post(() =>
                {
                    item.Progress = 0;
                    item.IsProgressIndeterminate = total <= 0;
                    item.StatusText = total > 0 ? $"Starting — {FormatBytes(total)}" : "Starting…";
                    item.Status = DownloadStatus.Downloading;
                });

                using (Stream input = await response.Content.ReadAsStreamAsync(token))
                using (FileStream output = new(tempPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true))
                {
                    byte[] buffer = new byte[81920];
                    int read;

                    while ((read = await input.ReadAsync(buffer.AsMemory(), token)) > 0)
                    {
                        await output.WriteAsync(buffer.AsMemory(0, read), token);
                        received += read;

                        if (Environment.TickCount64 - lastProgressPost >= 250)
                        {
                            lastProgressPost = Environment.TickCount64;
                            PostProgress(item, received, total);
                        }
                    }

                    await output.FlushAsync(token);
                }

                PostProgress(item, received, total);

                // Verify the SHA-256 digest published by GitHub when one is available.
                if (!string.IsNullOrWhiteSpace(expectedSha256))
                {
                    string actual = await ComputeSha256Async(tempPath, token);

                    if (!string.Equals(actual, expectedSha256.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        TryDelete(tempPath);
                        SetFailed(item, "Checksum verification failed (SHA-256 mismatch). The download was discarded.");
                        return DownloadOutcome.Failed;
                    }
                }

                File.Move(tempPath, finalPath, overwrite: true);
                tempPath = null;

                string statusText = $"Completed — {FormatBytes(received)}";
                string launchPath = DownloadItem.IsLaunchableFile(finalPath) ? finalPath : string.Empty;

                // Zip archives are safe-extracted so homebrew shows up in the game library.
                if (string.Equals(Path.GetExtension(finalPath), ".zip", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        string extractDir = Path.Combine(
                            directory,
                            Path.GetFileNameWithoutExtension(fileName).TrimEnd('.') + " (extracted)");

                        int extracted = ExtractZipSafe(finalPath, extractDir);

                        string discovered = FindLaunchable(extractDir);
                        launchPath = string.IsNullOrEmpty(discovered) ? launchPath : discovered;

                        statusText = extracted > 0
                            ? $"Completed — extracted {extracted} file(s)"
                            : statusText;
                    }
                    catch (Exception ex)
                    {
                        Logger.Warning?.Print(LogClass.Application, $"Download extraction failed for \"{fileName}\": {ex.Message}");
                        statusText += " (archive extraction failed)";
                    }
                }

                string savedPath = finalPath;
                string discoveredLaunchPath = launchPath;

                Dispatcher.UIThread.Post(() =>
                {
                    item.SavePath = savedPath;
                    item.LaunchPath = discoveredLaunchPath;
                    item.Progress = 100;
                    item.IsProgressIndeterminate = false;
                    item.StatusText = statusText;
                    item.ErrorMessage = string.Empty;
                    item.Status = DownloadStatus.Completed;

                    ItemCompleted?.Invoke(item);
                });

                return DownloadOutcome.Completed;
            }
            catch (OperationCanceledException)
            {
                TryDelete(tempPath);
                MarkCanceled(item);
                return DownloadOutcome.Failed;
            }
            catch (Exception ex)
            {
                TryDelete(tempPath);
                SetFailed(item, FriendlyError(ex));
                return DownloadOutcome.Failed;
            }
        }

        private static string FindLaunchable(string directory)
        {
            try
            {
                if (!Directory.Exists(directory))
                {
                    return null;
                }

                foreach (string extension in DownloadItem.LaunchableExtensions)
                {
                    string match = Directory
                        .EnumerateFiles(directory, "*" + extension, SearchOption.AllDirectories)
                        .FirstOrDefault();

                    if (match != null)
                    {
                        return match;
                    }
                }
            }
            catch
            {
                // Discovery is best-effort; the user can still open the folder.
            }

            return null;
        }

        /// <summary>
        /// Extracts a zip archive while rejecting entries that would escape the
        /// destination directory (absolute paths or ".." traversal).
        /// </summary>
        private static int ExtractZipSafe(string zipPath, string destinationDirectory)
        {
            string destinationRoot = Path.GetFullPath(destinationDirectory)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            Directory.CreateDirectory(destinationRoot);

            int extracted = 0;

            using ZipFile zip = new(zipPath);

            foreach (ZipEntry entry in zip)
            {
                if (!entry.IsFile)
                {
                    continue;
                }

                string entryName = entry.Name.Replace('\\', '/');

                if (string.IsNullOrWhiteSpace(entryName) ||
                    entryName.StartsWith("/") || entryName.Contains("../") ||
                    string.Equals(entryName, "..", StringComparison.Ordinal))
                {
                    continue;
                }

                string outputPath = Path.GetFullPath(Path.Combine(destinationRoot, entryName));

                if (!outputPath.StartsWith(
                        destinationRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) &&
                    !outputPath.StartsWith(
                        destinationRoot + Path.AltDirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string outputFolder = Path.GetDirectoryName(outputPath);

                if (!string.IsNullOrEmpty(outputFolder))
                {
                    Directory.CreateDirectory(outputFolder);
                }

                using Stream input = zip.GetInputStream(entry);
                using FileStream output = File.Create(outputPath);
                input.CopyTo(output);

                extracted++;
            }

            return extracted;
        }

        private static async Task<string> ComputeSha256Async(string filePath, CancellationToken token)
        {
            using FileStream stream = File.OpenRead(filePath);
            using SHA256 sha256 = SHA256.Create();
            byte[] hash = await sha256.ComputeHashAsync(stream, token);
            return Convert.ToHexString(hash);
        }

        private static void PostProgress(DownloadItem item, long received, long total)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (total > 0)
                {
                    item.IsProgressIndeterminate = false;
                    item.Progress = Math.Clamp(received * 100.0 / total, 0, 100);
                    item.StatusText = $"{item.Progress:0}% — {FormatBytes(received)} of {FormatBytes(total)}";
                }
                else
                {
                    item.IsProgressIndeterminate = true;
                    item.Progress = 0;
                    item.StatusText = $"Downloaded {FormatBytes(received)}";
                }
            });
        }

        private static void PostState(
            DownloadItem item,
            DownloadStatus status,
            string statusText,
            double? progress,
            bool indeterminate)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (progress.HasValue)
                {
                    item.Progress = progress.Value;
                }

                item.IsProgressIndeterminate = indeterminate;
                item.StatusText = statusText;
                item.Status = status;
            });
        }

        private static void SetFailed(DownloadItem item, string message)
        {
            Logger.Error?.Print(LogClass.Application, $"Download \"{item.Title}\" failed: {message}");

            Dispatcher.UIThread.Post(() =>
            {
                if (item.Status == DownloadStatus.Completed)
                {
                    return;
                }

                item.IsProgressIndeterminate = false;
                item.ErrorMessage = message;
                item.StatusText = "Failed";
                item.Status = DownloadStatus.Failed;
            });
        }

        private static void MarkCanceled(DownloadItem item)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (item.Status is DownloadStatus.Completed or DownloadStatus.Failed)
                {
                    return;
                }

                item.IsProgressIndeterminate = false;
                item.ErrorMessage = string.Empty;
                item.StatusText = "Canceled";
                item.Status = DownloadStatus.Canceled;
            });
        }

        private static string FriendlyError(Exception ex)
        {
            if (ex is HttpRequestException httpEx)
            {
                if (httpEx.StatusCode == HttpStatusCode.NotFound)
                {
                    return "The file was not found (HTTP 404).";
                }

                if (httpEx.StatusCode is HttpStatusCode.Forbidden or HttpStatusCode.TooManyRequests)
                {
                    return "GitHub is rate-limiting requests from this network. Wait a few minutes and try again.";
                }

                if (httpEx.StatusCode != null)
                {
                    return $"Network error (HTTP {(int)httpEx.StatusCode}).";
                }

                return "Network error — check your internet connection.";
            }

            if (ex is IOException)
            {
                return $"Disk error — {ex.Message}";
            }

            if (ex is UnauthorizedAccessException)
            {
                return "Permission denied — OmniEMU could not write to the downloads folder.";
            }

            if (ex is OperationCanceledException)
            {
                return "The download was canceled.";
            }

            return ex.Message;
        }

        private static string BuildUserAgent()
        {
            string version = Program.Version ?? "0.1.0";

            int plusIndex = version.IndexOf('+');

            if (plusIndex >= 0)
            {
                version = version.Substring(0, plusIndex);
            }

            return $"OmniEMU/{version}";
        }

        private static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return string.Empty;
            }

            string name = fileName.Trim();
            name = Path.GetFileName(name);

            foreach (char invalid in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(invalid, '_');
            }

            name = name.Trim(' ', '.');

            return name;
        }

        private static string GetUniquePath(string directory, string fileName)
        {
            string name = Path.GetFileName(fileName);
            string extension = Path.GetExtension(name);
            string baseName = Path.GetFileNameWithoutExtension(name);

            string candidate = Path.Combine(directory, name);
            int counter = 1;

            while (File.Exists(candidate) || File.Exists(candidate + ".part"))
            {
                counter++;
                candidate = Path.Combine(directory, $"{baseName} ({counter}){extension}");
            }

            return candidate;
        }

        private static void TryDelete(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch
            {
                // Best-effort cleanup of partial files.
            }
        }

        private static string FormatBytes(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            double size = bytes;
            int unit = 0;

            while (size >= 1024 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }

            return unit == 0 ? $"{size:0} {units[unit]}" : $"{size:0.0} {units[unit]}";
        }
    }
}
