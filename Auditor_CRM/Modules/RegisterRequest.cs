﻿namespace Auditor_ManagerOnline.Models
{
    public class RegisterRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User";
    }
}
