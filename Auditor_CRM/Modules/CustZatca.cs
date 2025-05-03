using System.ComponentModel.DataAnnotations;

namespace Auditor_CRM.Modules
{
    public class CustZatca
    {
        [Required]
        // تحديد المفتاح الأساسي
        public int Id { get; set; }  // يجب أن يكون للمفتاح الأساسي نوع بيانات صحيح مثل int أو long
        public string ClientName { get; set; }

        public Boolean IsActive { get; set; }

        // يمكن إضافة هذا التحقق هنا
        [Required]
        public DateTime startDate { get; set; }
    }
}
