using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using UMS.Models.Entities;

namespace UMS.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }
        // DbSets for your entities
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Designation> Designations { get; set; }
        public DbSet<ManagerAttendance> ManagerAttendances { get; set; }
        public DbSet<RefreshTokenManager> RefreshTokenManagers { get; set; }
        public DbSet<RefreshTokenEmployee> RefreshTokenEmployees { get; set; }
        public DbSet<LoginVerificationOTP> LoginVerificationOTPs { get; set; }
        public DbSet<Admin> Admins { get; set; }
        // Add other DbSets as needed
    }
}
