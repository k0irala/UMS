namespace UMS;

public class DataTableRequest
{
    public int Take { get; set; }
    public int Skip { get; set; }
    public string OrderColumn { get; set; } = string.Empty;
    public string OrderDirection { get; set; } = string.Empty;
}