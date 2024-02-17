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
    }
}