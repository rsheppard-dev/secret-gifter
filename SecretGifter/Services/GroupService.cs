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
                Group? group = await _context.Groups.FindAsync(groupId);

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

        public Task<Group> UpdateGroupAsync(Group group)
        {
            throw new NotImplementedException();
        }
    }
}