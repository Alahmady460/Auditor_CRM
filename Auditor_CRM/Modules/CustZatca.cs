using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auditor_CRM.Modules
{
    [Table("custzatca", Schema = "crmdb")]
    public class custzatca
    {
        [Required]
        [Key]
        // تحديد المفتاح الأساسي
        public int Id { get; set; }  // يجب أن يكون للمفتاح الأساسي نوع بيانات صحيح مثل int أو long
        public string ClientName { get; set; }

        public Boolean IsActive { get; set; }

        // يمكن إضافة هذا التحقق هنا
        [Required]
        public DateTime startDate { get; set; }
    }
}
