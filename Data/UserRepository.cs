namespace Eventure_ASP.Data
{
    using Eventure_ASP.Models;
    using Microsoft.EntityFrameworkCore;
    using System.Threading.Tasks;

    public class UserRepository
    {
        private readonly EtsDbContext _context;

        public UserRepository(EtsDbContext context)
        {
            _context = context;
        }

        public User GetUserByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username); // Fetch user by username
        }

        public User GetUserById(int id)
        {
            return _context.Users.Find(id); // Adjust based on your actual DbContext setup
        }
    }
}
