using Microsoft.EntityFrameworkCore;
using SecretGifter.Data;
using SecretGifter.Models;
using SecretGifter.Services.Interfaces;

namespace SecretGifter.Services
{
    public class GroupService(ApplicationDbContext context, ILogger<GroupService> logger, HttpContext httpContext) : IGroupService
    {
        private readonly ApplicationDbContext _context = context;
        private readonly HttpContext _httpContext = httpContext;
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

                UserGroup userGroup = new()
                {
                    GroupId = groupId,
                    UserId = userId,
                    IsAdmin = group.Members.Count == 0
                };

                _context.UserGroup.Add(userGroup);
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

        public async Task<Group> CreateGroupAsync(Group group, string userId)
        {
            try
            {
                ApplicationUser? user = await _context.Users.FindAsync(userId);

                if (user is null)
                {
                    _logger.LogWarning("User {userId} not found", userId);
                    throw new ArgumentException("User not found");
                }
                
                _context.Groups.Add(group);
                await _context.SaveChangesAsync();

                var userGroup = new UserGroup
                {
                    GroupId = group.Id,
                    UserId = userId,
                    IsAdmin = true
                };

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
                Group? group = await GetGroupByIdAsync(groupId);

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

        public async Task<bool> DoesGroupExistAsync(int groupId)
        {
            try
            {
                Group? group = await GetGroupByIdAsync(groupId);

                return group is not null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking if group {groupId} exists", groupId);
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
                IQueryable<ApplicationUser> members = _context.Groups
                    .Include(g => g.Members) 
                    .Where(g => g.Id == groupId)   
                    .SelectMany(g => g.Members);

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

                UserGroup? userGroup = await _context.UserGroup
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
                Group? group = await GetGroupByIdAsync(groupId);

                if (group is null)
                {
                    _logger.LogWarning("Group {groupId} not found", groupId);
                    return false;
                }

                return group.Members.Any(u => u.Id == userId);
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
                UserGroup? userGroup = await _context.UserGroup
                    .FirstOrDefaultAsync(gu => gu.GroupId == groupId && gu.UserId == userId);

                if (userGroup is null)
                {
                    _logger.LogWarning("User {userId} not found in group {groupId}", userId, groupId);
                    return false;
                }

                userGroup.IsAdmin = true;
                
                _context.UserGroup.Update(userGroup);
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
                UserGroup? userGroup = await _context.UserGroup
                    .FirstOrDefaultAsync(gu => gu.GroupId == groupId && gu.UserId == userId);

                if (userGroup is null)
                {
                    _logger.LogWarning("User {userId} not found in group {groupId}", userId, groupId);
                    return false;
                }

                // check if user is the only admin
                if (userGroup.IsAdmin && _context.UserGroup.Count(gu => gu.GroupId == groupId && gu.IsAdmin) == 1)
                {
                    _logger.LogWarning("User {userId} is the only admin for group {groupId}", userId, groupId);
                    return false;
                }

                userGroup.IsAdmin = false;
                
                _context.UserGroup.Update(userGroup);
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
                Group? group = await GetGroupByIdAsync(groupId);

                if (group is null)
                {
                    _logger.LogWarning("Group {groupId} not found", groupId);
                    return false;
                }

                ApplicationUser? user = group.Members.FirstOrDefault(u => u.Id == userId);

                if (user is null)
                {
                    _logger.LogWarning("User {userId} not found in group {groupId}", userId, groupId);
                    return false;
                }

                group.Members.Remove(user);
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