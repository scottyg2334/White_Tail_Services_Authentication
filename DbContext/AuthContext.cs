using Microsoft.EntityFrameworkCore;
using AuthorizationServer.Models;



namespace AuthorizationServer.DbContext
{
    public class AuthContext : Microsoft.EntityFrameworkCore.DbContext
    {

        public AuthContext(DbContextOptions<AuthContext> options)
            : base(options)
        {
            
        }
        public DbSet<AuthUserModel> AuthUsers { get; set; }
   
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=localhost;Database=users;Username=scott;Password=password;Port=5433");


    }


    
        
    
}