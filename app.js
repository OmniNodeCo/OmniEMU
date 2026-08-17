const games = [
  { name: 'Pocket Rally', platform: 'Android', meta: 'Played 12 min ago', icon: 'R', cover: 'rally', favorite: true, recent: true },
  { name: 'Monument Valley', platform: 'iPhone', meta: 'Played yesterday', icon: 'M', cover: 'valley', favorite: true, recent: true },
  { name: 'Pixel Quest', platform: 'Nintendo', meta: 'Played 3 days ago', icon: 'P', cover: 'quest', favorite: false, recent: true },
  { name: 'Neon Drift', platform: 'Nintendo', meta: 'Never played', icon: 'N', cover: 'neon', favorite: false, recent: false }
];

let activeDevice = 'android';
let activeGame = games[0];
let activeFilter = 'all';
let powered = true;
let elapsedSeconds = 0;
let playerPosition = 15;

const $ = (selector) => document.querySelector(selector);
const $$ = (selector) => [...document.querySelectorAll(selector)];
const gameList = $('#gameList');

function renderGames() {
  const search = $('#searchInput').value.toLowerCase();
  const shown = games.filter(game => {
    const matchesSearch = game.name.toLowerCase().includes(search) || game.platform.toLowerCase().includes(search);
    const matchesFilter = activeFilter === 'all' || (activeFilter === 'recent' && game.recent) || (activeFilter === 'favorites' && game.favorite);
    return matchesSearch && matchesFilter;
  });
  gameList.innerHTML = shown.map(game => `
    <button class="game-card ${activeGame === game ? 'active' : ''}" data-game="${game.name}">
      <span class="cover ${game.cover}">${game.icon}</span>
      <span class="game-info"><strong>${game.name}</strong><small>${game.platform} · ${game.meta}</small></span>
      <span class="favorite ${game.favorite ? 'on' : ''}" data-favorite="${game.name}">${game.favorite ? '★' : '☆'}</span>
    </button>`).join('') || '<p style="color:#647388;font-size:11px;text-align:center;padding:25px 0">No games found</p>';

  $$('.game-card').forEach(card => card.addEventListener('click', () => {
    activeGame = games.find(g => g.name === card.dataset.game);
    renderGames();
    toast(`${activeGame.name} selected`);
  }));
  $$('[data-favorite]').forEach(button => button.addEventListener('click', event => {
    event.stopPropagation();
    const game = games.find(g => g.name === button.dataset.favorite);
    game.favorite = !game.favorite;
    renderGames();
  }));
}

function setDevice(device) {
  activeDevice = device;
  $$('.device-tab').forEach(tab => tab.classList.toggle('active', tab.dataset.device === device));
  const phone = $('#phoneFrame');
  const console = $('#switchFrame');
  const home = $('#homeContent');
  const running = $('#runningApp');
  running.classList.add('hidden');
  home.classList.remove('hidden');
  phone.classList.toggle('hidden', device === 'nintendo');
  console.classList.toggle('hidden', device !== 'nintendo');
  phone.classList.remove('rotated');
  $('#rotateButton').disabled = device === 'nintendo';

  const data = {
    android: { status: 'Pixel 8 Pro · Android 14', name: 'Pixel 8 Pro', meta: '1344 × 2992 · Android 14', frame: 'android-frame' },
    iphone: { status: 'iPhone 15 Pro · iOS 18', name: 'iPhone 15 Pro', meta: '1179 × 2556 · iOS 18', frame: 'iphone-frame' },
    nintendo: { status: 'Nintendo Switch · Handheld', name: 'Nintendo Switch', meta: '1280 × 720 · Handheld mode', frame: 'switch' }
  }[device];
  $('#deviceStatus').textContent = data.status;
  $('#profileName').textContent = data.name;
  $('#profileMeta').textContent = data.meta;
  $('.profile-device-icon').textContent = device === 'nintendo' ? '▭' : '▯';
  $('#phoneScreen .phone-wallpaper').className = `phone-wallpaper ${device === 'iphone' ? 'iphone-wallpaper' : 'android-wallpaper'}`;
  document.body.dataset.device = device;
  toast(`${data.name} connected`);
}

function launchGame() {
  powered = true;
  if (activeDevice === 'nintendo') {
    $('#consoleTitle').classList.remove('hidden');
    $('#consoleTitle strong').textContent = activeGame.platform === 'Nintendo' ? activeGame.name.toUpperCase() : 'PIXEL QUEST';
    toast(`Launching ${activeGame.name} on Nintendo`);
    return;
  }
  $('#homeContent').classList.add('hidden');
  $('#runningApp').classList.remove('hidden');
  $('#runningTitle').textContent = activeGame.name;
  $('#runningSubtitle').textContent = `Running on ${activeDevice === 'iphone' ? 'iOS' : 'Android'}`;
  $('#runningHero').textContent = activeGame.icon;
  $('#runningHero').className = `running-hero cover ${activeGame.cover}`;
  toast(`Launching ${activeGame.name}`);
}

function goHome() {
  if (activeDevice === 'nintendo') {
    $('#consoleTitle').classList.remove('hidden');
  } else {
    $('#runningApp').classList.add('hidden');
    $('#homeContent').classList.remove('hidden');
  }
}

function toast(message) {
  const node = $('#toast');
  node.querySelector('p').textContent = message;
  node.classList.add('show');
  clearTimeout(window.toastTimer);
  window.toastTimer = setTimeout(() => node.classList.remove('show'), 2200);
}

$$('.device-tab').forEach(tab => tab.addEventListener('click', () => setDevice(tab.dataset.device)));
$$('.filter').forEach(filter => filter.addEventListener('click', () => {
  activeFilter = filter.dataset.filter;
  $$('.filter').forEach(f => f.classList.toggle('active', f === filter));
  renderGames();
}));
$('#searchInput').addEventListener('input', renderGames);
document.addEventListener('keydown', event => {
  if ((event.metaKey || event.ctrlKey) && event.key.toLowerCase() === 'k') {
    event.preventDefault();
    $('#searchInput').focus();
  }
  if (activeDevice === 'nintendo') {
    if (event.key === 'ArrowLeft') movePlayer(-6);
    if (event.key === 'ArrowRight') movePlayer(6);
    if (event.key === 'ArrowUp') jumpPlayer();
  }
});

function importGame() { $('#fileInput').click(); }
$('#addGame').addEventListener('click', importGame);
$('#importCard').addEventListener('click', importGame);
$('#fileInput').addEventListener('change', event => {
  const file = event.target.files[0];
  if (!file) return;
  const name = file.name.replace(/\.[^.]+$/, '').replace(/[-_]/g, ' ');
  const ext = file.name.split('.').pop().toLowerCase();
  const platform = ['apk'].includes(ext) ? 'Android' : ['ipa'].includes(ext) ? 'iPhone' : 'Nintendo';
  const game = { name, platform, meta: 'Just imported', icon: name[0].toUpperCase(), cover: 'custom', favorite: false, recent: true };
  games.unshift(game); activeGame = game; renderGames(); toast(`${file.name} added to library`);
});

$('#launchButton').addEventListener('click', launchGame);
$('#playDemo').addEventListener('click', () => toast('Demo session started'));
$('#homeControl').addEventListener('click', goHome);
$('#backControl').addEventListener('click', goHome);
$('#overviewControl').addEventListener('click', () => toast('Recent apps opened'));
$('#rotateButton').addEventListener('click', () => $('#phoneFrame').classList.toggle('rotated'));
$('#screenshotButton').addEventListener('click', () => {
  $('#stage').classList.add('flash');
  setTimeout(() => $('#stage').classList.remove('flash'), 400);
  toast('Screenshot captured');
});
$('#fullscreenButton').addEventListener('click', () => {
  if (!document.fullscreenElement) $('#stage').requestFullscreen?.(); else document.exitFullscreen?.();
});
$('#powerControl').addEventListener('click', () => {
  powered = !powered;
  const screen = activeDevice === 'nintendo' ? $('.console-screen') : $('#phoneScreen');
  screen.style.filter = powered ? '' : 'brightness(0)';
  toast(powered ? 'Device powered on' : 'Device powered off');
});
$('#volumeUp').addEventListener('click', () => toast('Volume 70%'));
$('#volumeDown').addEventListener('click', () => toast('Volume 50%'));
$('#soundToggle').addEventListener('click', event => {
  event.currentTarget.classList.toggle('muted');
  toast(event.currentTarget.classList.contains('muted') ? 'Sound muted' : 'Sound enabled');
});
$$('.app-icon').forEach(app => app.addEventListener('click', () => {
  $('#homeContent').classList.add('hidden');
  $('#runningApp').classList.remove('hidden');
  $('#runningTitle').textContent = app.dataset.app;
  $('#runningSubtitle').textContent = 'System app preview';
  $('#runningHero').textContent = app.querySelector('i').textContent;
}));

$('#profileButton').addEventListener('click', () => $('#profileMenu').classList.toggle('hidden'));
$$('#profileMenu button').forEach(option => option.addEventListener('click', () => {
  const [name, meta] = option.dataset.profile.split('|');
  $('#profileName').textContent = name; $('#profileMeta').textContent = meta;
  $('#deviceStatus').textContent = `${name} · ${meta.split('·')[1].trim()}`;
  $('#profileMenu').classList.add('hidden'); toast(`${name} profile loaded`);
}));
$('#fpsSlider').addEventListener('input', event => $('#fpsLabel').textContent = `${event.target.value} FPS`);

$('#settingsButton').addEventListener('click', () => $('#settingsModal').classList.remove('hidden'));
$('#modalClose').addEventListener('click', () => $('#settingsModal').classList.add('hidden'));
$('#saveSettings').addEventListener('click', () => { $('#settingsModal').classList.add('hidden'); toast('Settings saved'); });
$('#settingsModal').addEventListener('click', event => { if (event.target === $('#settingsModal')) $('#settingsModal').classList.add('hidden'); });

function movePlayer(amount) {
  playerPosition = Math.max(3, Math.min(92, playerPosition + amount));
  $('#pixelPlayer').style.left = `${playerPosition}%`;
}
function jumpPlayer() {
  $('#pixelPlayer').style.bottom = '36%';
  setTimeout(() => $('#pixelPlayer').style.bottom = '20%', 280);
}
$$('[data-dir]').forEach(button => button.addEventListener('click', () => {
  if (button.dataset.dir === 'left') movePlayer(-6);
  if (button.dataset.dir === 'right') movePlayer(6);
  if (button.dataset.dir === 'up') jumpPlayer();
}));
$('#consoleStart').addEventListener('click', () => { $('#consoleTitle').classList.add('hidden'); toast('Use the D-pad or arrow keys to move'); });
$$('.abxy button').forEach(button => button.addEventListener('click', jumpPlayer));

setInterval(() => {
  elapsedSeconds++;
  const h = String(Math.floor(elapsedSeconds / 3600)).padStart(2, '0');
  const m = String(Math.floor(elapsedSeconds / 60) % 60).padStart(2, '0');
  const s = String(elapsedSeconds % 60).padStart(2, '0');
  $('#elapsed').textContent = `${h}:${m}:${s}`;
  const cpu = 14 + Math.floor(Math.random() * 10);
  $('#cpuMetric').textContent = `${cpu}%`;
  $('#cpuMetric').nextElementSibling.style.setProperty('--value', `${cpu}%`);
}, 1000);

const now = new Date();
$('#phoneTime').textContent = now.toLocaleTimeString([], { hour: 'numeric', minute: '2-digit' });
renderGames();
