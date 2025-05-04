using Auditor_CRM.Services;
using AuditorManager.Moduls;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Auditor_CRM.Modules;
using System.Globalization;

[Route("api/Customer")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    // ✅ Constructor Injection
    public CustomerController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customers>>> GetAll()
    {
        var data = await _context.Customers.ToListAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customers>> Get(int id)
    {
        var data = await _context.Customers.FindAsync(id);
        return data == null ? NotFound() : Ok(data);
    }

    [HttpGet("expired")]
    public async Task<IActionResult> GetExpiredLicenses()
    {
        var oneYearAgo = DateTime.Now;
        var expiredClients = await _context.Customers
            .Where(x => x.EndDate < oneYearAgo)
            .ToListAsync();

        return Ok(expiredClients);
    }

    [HttpGet("getLicenceEndDate/{CompanyName}")]
    public async Task<IActionResult> GetExpiredLicensesBySyncCode(string CompanyName)
    {
        var expiredClients = await _context.Customers
            .Where(x => x.CompanyName == CompanyName)
            .ToListAsync();

        return expiredClients.Any() ? Ok(expiredClients) : NotFound("⚠️ لا يوجد عملاء منتهية تراخيصهم لهذا الكود.");
    }

    [HttpPost("Cust")]
    public async Task<IActionResult> UpsertCompany([FromBody] Customers data)
    {
        if (data == null || string.IsNullOrEmpty(data.CompanyName))
            return BadRequest("❌ البيانات غير صحيحة!");

        var existingCompany = await _context.Customers
            .FirstOrDefaultAsync(c => c.CompanyName == data.CompanyName);

        if (existingCompany != null)
        {
            existingCompany.LastConnect = data.LastConnect;
            existingCompany.LicenseDate = data.LicenseDate;
            existingCompany.LastEntry = data.LastEntry;
            _context.Customers.Update(existingCompany);
        }
        else
        {
            data.EndDate = data.LicenseDate.AddYears(1);
            _context.Customers.Add(data);
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "✅ تم الحفظ بنجاح.", data });
    }
    [HttpPost("Zatca")]
    public async Task<IActionResult> UpsertCustZatc([FromBody] custzatca data)
    {
        // تحقق من البيانات المدخلة
        if (data == null || string.IsNullOrEmpty(data.ClientName))
            return BadRequest("❌ البيانات غير صحيحة!");

        // تحقق من التاريخ المدخل
        if (data.startDate == default(DateTime))
        {
            return BadRequest(new { message = "⚠️ التاريخ المدخل غير صالح." });
        }

        // تحقق من وجود الشركة مسبقًا
        var existingCompany = await _context.custzatca
            .FirstOrDefaultAsync(c => c.ClientName == data.ClientName);

        if (existingCompany != null)
        {
            return Conflict(new { message = "⚠️ هذا العميل موجود بالفعل." });
        }
        else
        {
            _context.custzatca.Add(data);
        }

        // حفظ البيانات في قاعدة البيانات
        try
        {
            await _context.SaveChangesAsync();
            return Ok(new { message = "✅ تم الحفظ بنجاح.", data });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "❌ فشل في حفظ البيانات.", error = ex.Message });
        }
    }




    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var existingData = await _context.Customers.FindAsync(id);
        if (existingData == null) return NotFound();

        _context.Customers.Remove(existingData);
        await _context.SaveChangesAsync();
        return Ok(new { message = "🗑️ تم الحذف بنجاح" });
    }

    [HttpPut("updateEndDate/{companyName}")]
    public async Task<IActionResult> UpdateEndDate(string companyName, [FromBody] UpdateEndDateRequest request)
    {
        var record = await _context.Customers.FirstOrDefaultAsync(r => r.CompanyName == companyName);
        if (record == null) return NotFound("⚠️ السجل غير موجود.");

        record.EndDate = request.EndDate;
        await _context.SaveChangesAsync();

        return Ok(new { newEndDate = record.EndDate });
    }

    [HttpPost("sendEmailNotification")]
    public IActionResult SendEmailNotification([FromBody] EmailRequest request)
    {
        if (string.IsNullOrEmpty(request.Email))
            return BadRequest("❌ البريد الإلكتروني غير متوفر!");

        try
        {
            var smtpClient = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("Alahmady460@outlook.sa", "Ww778899@Ww"),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("Alahmady460@outlook.sa"),
                Subject = "إشعار تجديد الاشتراك",
                Body = "📢 تنبيه: يرجى تجديد اشتراكك!",
                IsBodyHtml = false,
            };

            mailMessage.To.Add(request.Email);
            smtpClient.Send(mailMessage);

            return Ok(new { message = "✅ تم إرسال الإشعار بنجاح!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "❌ خطأ في إرسال البريد الإلكتروني: " + ex.Message });
        }
    }
    [HttpGet("custzatca")]
    public async Task<IActionResult> Getcustzatca()
    {
        var clientzatca = await _context.custzatca
            .ToListAsync();

        return Ok(clientzatca);
    }


    public class UpdateEndDateRequest
    {
        public DateTime EndDate { get; set; }
    }
}
