using Auditor_CRM.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Auditor_CRM.Controllers
{
    public class SyncDataController : ControllerBase
    {
    }
    [Route("api/sync")]
    [ApiController]
    public class SyncController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SyncController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ جلب جميع بيانات المزامنة
        [HttpGet("syncdata")]
        public async Task<ActionResult<IEnumerable<SyncData>>> GetSyncData()
        {
            return await _context.SyncDatas.ToListAsync();
        }

        // ✅ إدخال أو تحديث بيانات المزامنة
        [HttpPost("syncdata")]
        public async Task<IActionResult> listsync([FromBody] List<SyncData> syncDataList)
        {
            if (syncDataList == null || syncDataList.Count == 0)
                return BadRequest("⚠️ البيانات غير صحيحة.");

            foreach (var data in syncDataList)
            {
                var existingSync = await _context.SyncDatas
                    .FirstOrDefaultAsync(s => s.SyncCode == data.SyncCode);

                if (existingSync != null)
                {
                    existingSync.StartDate = data.StartDate;
                    existingSync.EndDate = data.EndDate;
                    existingSync.Status = data.Status;
                    _context.SyncDatas.Update(existingSync);
                }
                else
                {
                    _context.SyncDatas.Add(data);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "✅ تم تخزين البيانات بنجاح", data = syncDataList });
        }

        // ✅ تحديث تاريخ الانتهاء بناءً على syncCode
        [HttpPut("updateEndDateSync")]
        public async Task<IActionResult> UpdateEndDate([FromQuery] string syncCode, [FromQuery] string updatedData) // تمرير البيانات عبر الـ URL
        {
            // محاولة تحويل النص إلى تاريخ
            if (!DateTime.TryParse(updatedData, out DateTime parsedDate))
            {
                return BadRequest(new { message = "⚠️ التاريخ المدخل غير صالح." });
            }

            // التحقق من أن التاريخ ليس في الماضي
            if (parsedDate < DateTime.Today)
            {
                return BadRequest(new { message = "⚠️ التاريخ يجب أن يكون في المستقبل." });
            }

            var record = await _context.SyncDatas.FirstOrDefaultAsync(r => r.SyncCode == syncCode);
            if (record == null)
            {
                return NotFound(new { message = "⚠️ السجل غير موجود." });
            }

            record.EndDate = parsedDate; // تحديث تاريخ الانتهاء
            await _context.SaveChangesAsync();
            return Ok(new { message = "✅ تم تحديث تاريخ الانتهاء بنجاح." });
        }

        // ✅ تشغيل/إيقاف المزامنة بناءً على الحالة
        [HttpPut("ToggleSync")]
        public async Task<IActionResult> ToggleSync([FromQuery] string syncCode, [FromQuery] bool syncStatus)
        {
            var record = await _context.SyncDatas.FirstOrDefaultAsync(r => r.SyncCode == syncCode);
            if (record == null)
            {
                return NotFound(new { message = "⚠️ السجل غير موجود." });
            }

            // التحقق من تاريخ الانتهاء
            var now = DateTime.Today; // التاريخ الحالي بدون وقت
            var syncEndDate = new DateTime(record.EndDate.Year, record.EndDate.Month, record.EndDate.Day); // تحويل تاريخ الانتهاء إلى تاريخ بدون وقت

            // إذا كانت الحالة جديدة لا يمكن تشغيلها/إيقافها بناءً على تاريخ الانتهاء
            if (syncStatus && syncEndDate < now)
            {
                return BadRequest(new { message = "⚠️ لا يمكن تشغيل المزامنة لأن تاريخ الانتهاء أقل من تاريخ اليوم. يرجى تجديد التاريخ أولاً." });
            }

            if (!syncStatus && syncEndDate < now)
            {
                return BadRequest(new { message = "⚠️ لا يمكن إيقاف المزامنة لأن تاريخ الانتهاء قد انتهى بالفعل. يرجى تجديد التاريخ أولاً." });
            }

            // تحديث حالة المزامنة
            record.Status = syncStatus;
            await _context.SaveChangesAsync();

            // إضافة سجل لنجاح العملية
            return Ok(new { status = record.Status });
        }




        // ✅ استدعاء تاريخ الانتهاء
        [HttpGet("getEndDate/{syncCode}")]
        public async Task<IActionResult> GetEndDate(string syncCode)
        {
            var record = await _context.SyncDatas.FirstOrDefaultAsync(r => r.SyncCode == syncCode);
            if (record == null)
                return NotFound("⚠️ السجل غير موجود.");

            return Ok(new
            {
                endDate = record.EndDate,
                Status = record.Status
            });
        }

    }
}
