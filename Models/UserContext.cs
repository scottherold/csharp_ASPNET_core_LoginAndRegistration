using Microsoft.EntityFrameworkCore;

namespace LoginAndRegistration.Models
{
    public class UserContext : DbContext
    {
        // Creates the Usercontext object ot use for querying
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        // Creates a list of queried data
        public DbSet<User> Users { get; set; }
    }
}