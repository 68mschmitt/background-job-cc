namespace BackgroundJobCodingChallenge.Messages;

// Messages/EmployeeUploadMessage.cs
public class EmployeeUploadMessage
{
    public string TenantId { get; set; } = default!;
    public int CsvRowId { get; set; }
}
