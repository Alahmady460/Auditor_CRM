using Auditor_CRM.Services;
using Auditor_ManagerOnline.Models;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
namespace Auditor_CRM.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public UserController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // ✅ تسجيل الدخول (إرجاع JWT Token)
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {

      
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.UserName) || string.IsNullOrEmpty(loginRequest.Password))
                return BadRequest("❌ بيانات تسجيل الدخول غير صحيحة.");

            // 🔎 البحث عن المستخدم في قاعدة البيانات
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == loginRequest.UserName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
                return BadRequest("❌ بيانات تسجيل الدخول غير صحيحة.");

                // 🔥 إنشاء التوكن
                var token = GenerateJwtToken(user);

            return Ok(new { token, userName = user.UserName, role = user.Role });
            }
            catch (Exception ex)
            {

                return BadRequest("❌ بيانات تسجيل الدخول غير صحيحة." + ex.Message);
            }
        }

        // ✅ إنشاء مستخدم جديد
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            if (registerRequest == null || string.IsNullOrEmpty(registerRequest.UserName) || string.IsNullOrEmpty(registerRequest.Password))
                return BadRequest("❌ بيانات التسجيل غير صحيحة.");

            if (await _context.Users.AnyAsync(u => u.UserName == registerRequest.UserName))
                return BadRequest("⚠️ اسم المستخدم موجود بالفعل.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password);

            var newUser = new UserModel
            {
                UserName = registerRequest.UserName,
                PasswordHash = hashedPassword,
                Role = registerRequest.Role ?? "User"
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ تم إنشاء الحساب بنجاح." });
        }

        [HttpPut("updatePassword/{id}")]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] UpdatePasswordDto model)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound(new { message = "المستخدم غير موجود" });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password); // تشفير كلمة المرور
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم تحديث كلمة المرور بنجاح!" });
        }


        [HttpGet("GetUser")]
        public async Task<ActionResult<UserModel>> GetUsers()
        {
            var data = await _context.Users.ToListAsync();
            if (data == null)
            {
                return NotFound();  // 🔴 يعيد 404 إذا لم توجد البيانات
            }
            return Ok(data);        // 🟢 يعيد 200 مع البيانات إذا وجدت
        }
        [HttpDelete("delete/{Id}")]
        public async Task<IActionResult> DeleteUser(int Id)
        {
            var user = await _context.Users.FindAsync(Id);
            if (user == null)
            {
                return NotFound("⚠️ المستخدم غير موجود.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ تم حذف المستخدم بنجاح." });
        }


        // 🔐 دالة لإنشاء التوكن
        private string GenerateJwtToken(UserModel user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.UtcNow.AddHours(1), // ⏳ مدة صلاحية التوكن
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
