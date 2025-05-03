using Auditor_CRM.Modules;
using Auditor_ManagerOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace Auditor_CRM.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Customers> Customers { get; set; }
        public DbSet<SyncData> SyncDatas { get; set; }
        public DbSet<UserModel> Users { get; set; } // ✅ إضافة جدول المستخدمين
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<CustZatca> custzatca { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserModel>().HasIndex(u => u.UserName).IsUnique(); // 🔹 منع تكرار اسم المستخدم
            // تعيين المفتاح الأساسي (Primary Key) إلى Id (EF Core سيولد هذا تلقائيًا)
            modelBuilder.Entity<CustZatca>().HasKey(c => c.Id);
        }    
    }
}
