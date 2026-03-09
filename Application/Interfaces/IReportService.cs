namespace SwiftCart.Application.Interfaces;

public interface IReportService
{
    (decimal TotalRevenue, int OrderCount, decimal AverageOrderValue) GetSalesSummary(DateTime? from, DateTime? to);
    List<(int ProductId, string ProductName, int QuantitySold, decimal Revenue)> GetTopProducts(int limit);
    List<(string PeriodLabel, decimal Revenue, int OrderCount)> GetRevenueByPeriod(string period);
}
