using DotnetAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DotnetAPI.Data
{
    public class DataContextEF : DbContext
    {
        private readonly IConfiguration _config;

        public DataContextEF(IConfiguration config)
        {
            _config = config;
        }

        public virtual DbSet<UserModel> Users { get; set; }
        public virtual DbSet<UserSalaryModel> UserSalary { get; set; }
        public virtual DbSet<UserJobInfoModel> UserJobInfo { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    _config.GetConnectionString("DefaultConnection"),
                    optionsBuilder => optionsBuilder.EnableRetryOnFailure()
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserModel>().HasKey(u => u.UserId);

            modelBuilder.Entity<UserSalaryModel>().HasKey(u => u.UserId);

            modelBuilder.Entity<UserJobInfoModel>().HasKey(u => u.UserId);
        }
    }
}
