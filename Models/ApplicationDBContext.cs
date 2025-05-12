using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TaskManagementSystem.Models
{
    public class ApplicationDBContext:IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base( options)
        {

        }    
        public DbSet<IdentityUserNew> IdentityUserNews { get; set; }    
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<TaskProgress> TasksProgress { get; set; } 

    }
}
