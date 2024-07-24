using Microsoft.EntityFrameworkCore;
namespace Student_mangment.Models
{
    public class StudentContext : DbContext
    {
        public StudentContext(DbContextOptions<StudentContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Contact> Contacts { get; set; }

    }
}
