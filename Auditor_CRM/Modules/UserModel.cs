using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auditor_ManagerOnline.Models
{
    [Table("users", Schema = "crmdb")]
    public class UserModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; } // ✅ اسم المستخدم

        [Required]
        public string PasswordHash { get; set; } // ✅ تخزين كلمة المرور بشكل مشفر

        public string Role { get; set; } = "User"; // ✅ يمكن أن يكون "Admin" أو "User"

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
    public class UpdatePasswordDto
    {
        public string Password { get; set; }
    }
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; } // مثل (Admin, Manager, User)
        public List<UserModel> Users { get; set; } // علاقة مع المستخدمين
    }

}
