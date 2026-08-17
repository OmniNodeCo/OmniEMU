using OmniEMU.Horizon.Common;

namespace OmniEMU.Horizon
{
    public static class LibHacResultExtensions
    {
        public static Result ToHorizonResult(this LibHac.Result result)
        {
            return new Result((int)result.Module, (int)result.Description);
        }
    }
}
