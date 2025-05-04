using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
[Table("syncData", Schema = "crmdb")]
public class SyncData
{
    [Key]
    public int Id { get; set; }
    public string ClientName { get; set; }
    public string SyncCode { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Boolean Status { get; set; }
}
