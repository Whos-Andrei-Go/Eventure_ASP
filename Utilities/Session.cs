using Eventure_ASP.Data;
using Eventure_ASP.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Eventure_ASP.Utilities
{
    public class Session
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserRepository _userRepository;

        public Session(IHttpContextAccessor httpContextAccessor, UserRepository userRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
        }

        private const string CurrentUserKey = "CurrentUser ";

        public void SetCurrentUser(User user)
        {
            _httpContextAccessor.HttpContext.Session.SetString(CurrentUserKey, user.Id.ToString());
        }

        public User GetCurrentUser()
        {
            var userIdString = _httpContextAccessor.HttpContext.Session.GetString(CurrentUserKey);
            if (userIdString != null)
            {
                var userId = int.Parse(userIdString);
                return _userRepository.GetUserById(userId);
            }
            return null;
        }

        public void ClearCurrentUser()
        {
            _httpContextAccessor.HttpContext.Session.Remove(CurrentUserKey);
        }

        public void Logout()
        {
            ClearCurrentUser();
        }
    }
}