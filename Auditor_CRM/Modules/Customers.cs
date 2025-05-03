public class Customers
{
    public int Id { get; set; }
    public string CompanyName { get; set; }
    public DateTime LastConnect { get; set; }  // ✅ تأكد أنه DateTime
    public string Mobile { get; set; }
    public string Email { get; set; }
    public string City { get; set; }           // ✅ تأكد من الحرف الكبير
    public string Salsman { get; set; }
    public int DeviceNo { get; set; }
    public DateTime LicenseDate { get; set; }  // تأريخ الترخيص
    public DateTime EndDate { get; set; } // ✅ تاريخ الانتهاء
    public string LastEntry { get; set; }
}
