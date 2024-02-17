using SecretGifter.Data;

namespace SecretGifter.Services.Interfaces
{
    public interface IGroupService
    {
        public Task<bool> IsUserGroupAdminAsync(int groupId, string userId);
        public Task<bool> IsUserInGroupAsync(int groupId, string userId);
        public IQueryable<ApplicationUser> GetQueryableGroupMembers(int groupId);
    }
}