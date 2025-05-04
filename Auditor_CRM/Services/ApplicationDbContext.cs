using Auditor_CRM.Modules;
using Auditor_ManagerOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace Auditor_CRM.Services
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { 
        
        
        }

        public DbSet<Customers> Customers { get; set; }
        public DbSet<SyncData> SyncDatas { get; set; }
        public DbSet<UserModel> Users { get; set; } // ✅ إضافة جدول المستخدمين
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<custzatca> custzatca { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // اجعل الافتراضي هو crmdb لجميع الجداول
            modelBuilder.HasDefaultSchema("crmdb");

            // ضبط اسم جدول SyncData ليطابق case-sensitive إن احتاج الأمر
            modelBuilder.Entity<SyncData>()
                .ToTable("syncDatas");

            // مثال: ضبط عمود ClientName ليكون مطلوباً وطوله الأقصى 100
            modelBuilder.Entity<SyncData>()
                .Property(s => s.ClientName)
                .IsRequired()
                .HasMaxLength(100);

            // فهرس على SyncCode لتحسين استعلامات البحث
            modelBuilder.Entity<SyncData>()
                .HasIndex(s => s.SyncCode);
        }

    }
}
