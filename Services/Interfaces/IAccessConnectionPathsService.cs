using System.Threading.Tasks;

namespace SecurityAPI.Services.Interfaces
{
    public interface IAccessConnectionPathsService
    {
        Task<(bool, string)> VerifyProfileAccess(long idProfile, long idScreen, string endpoint);
        Task<(bool, string)> VerifyUserAccess(long idProfile, long idScreen, string endpoint);
    }
}