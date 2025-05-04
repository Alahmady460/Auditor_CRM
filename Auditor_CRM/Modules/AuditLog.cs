using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auditor_ManagerOnline.Models
{
    [Table("auditLog", Schema = "crmdb")]
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } // ID المستخدم (إذا كان مسجلاً)
        public string Action { get; set; } // العملية التي تمت (إضافة، تعديل، حذف)
        public string ModelName { get; set; } // اسم الكيان المتأثر
        public int? ModelId { get; set; } // ID للعنصر المتأثر
        public string OldData { get; set; } // البيانات القديمة قبل التغيير
        public string NewData { get; set; } // البيانات الجديدة بعد التغيير
        public string IpAddress { get; set; } // عنوان الـ IP الخاص بالمستخدم
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // وقت العملية
    }
}
