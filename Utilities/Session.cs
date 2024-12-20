using Eventure_ASP.Data;
using Eventure_ASP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Eventure_ASP.Utilities
{
    public class Session
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserRepository _userRepository; // Add UserRepository

        public Session(IHttpContextAccessor httpContextAccessor, UserRepository userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository; // Initialize UserRepository
        }

        private const string CurrentUserKey = "CurrentUser "; // Fixed the constant declaration

        // Method to set the current user in session
        public void SetCurrentUser(User user)
        {
            _httpContextAccessor.HttpContext.Session.SetString(CurrentUserKey, user.Id.ToString()); // Store user ID
        }

        // Method to get the current user from session
        public User GetCurrentUser()
        {
            var userIdString = _httpContextAccessor.HttpContext.Session.GetString(CurrentUserKey);
            if (userIdString != null)
            {
                var userId = int.Parse(userIdString); // Assuming Id is an integer
                return _userRepository.GetUserById(userId); // Fetch user details by ID
            }
            return null;
        }

        // Method to clear the current user from session
        public void ClearCurrentUser()
        {
            _httpContextAccessor.HttpContext.Session.Remove(CurrentUserKey);
        }
    }
}