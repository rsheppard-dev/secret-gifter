using SecretGifter.Data;
using SecretGifter.Models;

namespace SecretGifter.Services.Interfaces
{
    public interface IGroupService
    {
        public Task<Group> CreateGroupAsync(Group group);
        public Task<Group?> GetGroupByIdAsync(int groupId);
        public Task<Group> UpdateGroupAsync(Group group);
        public Task<bool> DeleteGroupAsync(int groupId);
        public Task<bool> IsUserGroupAdminAsync(int groupId, string userId);
        public Task<bool> IsUserInGroupAsync(int groupId, string userId);
        public IQueryable<ApplicationUser> GetQueryableGroupMembers(int groupId);
    }
}