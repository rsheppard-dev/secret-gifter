using Microsoft.EntityFrameworkCore;
using SecretGifter.Data;
using SecretGifter.Models;
using SecretGifter.Services.Interfaces;

namespace SecretGifter.Services
{
    public class GroupService(ApplicationDbContext context, ILogger<GroupService> logger) : IGroupService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly ILogger<GroupService> _logger = logger;

        public async Task<bool> AddUserToGroupAsync(int groupId, string userId)
        {
            try
            {
                Group? group = await GetGroupByIdAsync(groupId);

                if (group is null)
                {
                    _logger.LogWarning("Group {groupId} not found", groupId);
                    return false;
                }

                ApplicationUser? user = await _context.Users.FindAsync(userId);

                if (user is null)
                {
                    _logger.LogWarning("User {userId} not found", userId);
                    return false;
                }

                if (await IsUserInGroupAsync(groupId, userId))
                {
                    _logger.LogWarning("User {userId} already in group {groupId}", userId, groupId);
                    return false;
                }

                GroupUser groupUser = new GroupUser
                {
                    GroupId = groupId,
                    UserId = userId,
                    IsAdmin = group.Members.Count == 0
                };

                _context.GroupUser.Add(groupUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {userId} added to group {groupId}", userId, groupId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding user {userId} to group {groupId}", userId, groupId);
                throw;
            }
        }

        public async Task<Group> CreateGroupAsync(Group group)
        {
            try
            {
                _context.Groups.Add(group);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Group {groupId} created", group.Id);
                return group;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating group {group}", group.Id);
                throw;
            }
        }

        public async Task<bool> DeleteGroupAsync(int groupId)
        {
            try
            {
                Group? group = await _context.Groups.FindAsync(groupId);

                if (group is null)
                {
                    _logger.LogWarning("Group {groupId} not found", groupId);
                    return false;
                }

                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Group {groupId} deleted", groupId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting group {groupId}", groupId);
                throw;
            }
        }

        public async Task<Group?> GetGroupByIdAsync(int groupId)
        {
            try
            {
                Group? group = await _context.Groups
                    .Include(g => g.Members)
                    .FirstOrDefaultAsync(g => g.Id == groupId);

                if (group is null)
                {
                    _logger.LogWarning("Group {groupId} not found", groupId);
                }

                return group;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting group {groupId}", groupId);
                throw;
            }
        }

        public IQueryable<ApplicationUser> GetQueryableGroupMembers(int groupId)
        {
            try
            {
                IQueryable<ApplicationUser> members = _context.GroupUser
                    .Where(gu => gu.GroupId == groupId)
                    .Include(gu => gu.User)
                    .Select(gu => gu.User);

                return members;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting members for group {groupId}", groupId);
                throw;
            }
        }

        public async Task<bool> IsUserGroupAdminAsync(int groupId, string userId)
        {
            try
            {
                GroupUser? userGroup = await _context.GroupUser
                    .FirstOrDefaultAsync(gu => gu.GroupId == groupId && gu.UserId == userId);

                return userGroup?.IsAdmin ?? false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if user {userId} is an admin for group {groupId}", userId, groupId);
                throw;
            }
        }

        public async Task<bool> IsUserInGroupAsync(int groupId, string userId)
        {
            try
            {
                GroupUser? userGroup = await _context.GroupUser
                    .FirstOrDefaultAsync(gu => gu.GroupId == groupId && gu.UserId == userId);

                return userGroup is not null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if user {userId} is in group {groupId}", userId, groupId);
                throw;
            }
        }

        public async Task<bool> MakeGroupAdminAsync(int groupId, string userId)
        {
            try
            {
                GroupUser? userGroup = await _context.GroupUser
                    .FirstOrDefaultAsync(gu => gu.GroupId == groupId && gu.UserId == userId);

                if (userGroup is null)
                {
                    _logger.LogWarning("User {userId} not found in group {groupId}", userId, groupId);
                    return false;
                }

                userGroup.IsAdmin = true;
                
                _context.GroupUser.Update(userGroup);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {userId} made admin for group {groupId}", userId, groupId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while making user {userId} an admin for group {groupId}", userId, groupId);
                throw;
            }
        }

        public async Task<bool> RemoveGroupAdminAsync(int groupId, string userId)
        {
            try
            {
                GroupUser? userGroup = await _context.GroupUser
                    .FirstOrDefaultAsync(gu => gu.GroupId == groupId && gu.UserId == userId);

                if (userGroup is null)
                {
                    _logger.LogWarning("User {userId} not found in group {groupId}", userId, groupId);
                    return false;
                }

                // check if user is the only admin
                if (userGroup.IsAdmin && _context.GroupUser.Count(gu => gu.GroupId == groupId && gu.IsAdmin) == 1)
                {
                    _logger.LogWarning("User {userId} is the only admin for group {groupId}", userId, groupId);
                    return false;
                }

                userGroup.IsAdmin = false;
                
                _context.GroupUser.Update(userGroup);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {userId} removed as admin for group {groupId}", userId, groupId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing user {userId} as admin for group {groupId}", userId, groupId);
                throw;
            }
        }

        public async Task<bool> RemoveUserFromGroupAsync(int groupId, string userId)
        {
            try
            {
                ApplicationUser? user = await _context.Users.FindAsync(userId);

                if (user is null)
                {
                    _logger.LogWarning("User {userId} not found", userId);
                    return false;
                }

                Group? group = await GetGroupByIdAsync(groupId);

                if (group is null)
                {
                    _logger.LogWarning("Group {groupId} not found", groupId);
                    return false;
                }

                GroupUser? userGroup = await _context.GroupUser
                    .FirstOrDefaultAsync(gu => gu.GroupId == groupId && gu.UserId == userId);

                if (userGroup is null)
                {
                    _logger.LogWarning("User {userId} not found in group {groupId}", userId, groupId);
                    return false;
                }

                _context.GroupUser.Remove(userGroup);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User {userId} removed from group {groupId}", userId, groupId);
                return true;   
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing user {userId} from group {groupId}", userId, groupId);
                throw;
            }
        }

        public async Task<Group> UpdateGroupAsync(Group group)
        {
            try
            {
                group.UpdatedAt = DateTime.UtcNow;
                
                _context.Groups.Update(group);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Group {groupId} updated", group.Id);
                return group;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating group {groupId}", group.Id);
                throw;
            }
        }
    }
}