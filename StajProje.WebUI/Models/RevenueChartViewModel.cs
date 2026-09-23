
public class RevenueChartViewModel
{
    public List<string> Labels { get; set; } = new();
    public List<int> Income { get; set; } = new();
    public List<int> Expense { get; set; } = new();
    public int TotalReservations { get; set; }
    public int ApprovedReservations { get; set; }
    public int CanceledReservations { get; set; }
}
