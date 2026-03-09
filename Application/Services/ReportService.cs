using System.Globalization;
using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;
using SwiftCart.Domain.Enums;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Application.Services;

public class ReportService : IReportService
{
    private readonly AppDb _db;

    public ReportService(AppDb db)
    {
        _db = db;
    }

    /// <summary>
    /// Returns total revenue, order count, and average order value for non-cancelled orders,
    /// optionally filtered by date range.
    /// </summary>
    public (decimal TotalRevenue, int OrderCount, decimal AverageOrderValue) GetSalesSummary(DateTime? from, DateTime? to)
    {
        var orders = _db.Orders
            .Where(o => o.Status != OrderStatus.Cancelled)
            .AsEnumerable();

        if (from.HasValue)
            orders = orders.Where(o => o.CreatedAt >= from.Value);
        if (to.HasValue)
            orders = orders.Where(o => o.CreatedAt <= to.Value);

        var orderList = orders.ToList();
        if (orderList.Count == 0)
            return (0, 0, 0);

        decimal totalRevenue = orderList.Sum(o => o.TotalAmount);
        int orderCount = orderList.Count;
        decimal avg = totalRevenue / orderCount;
        return (totalRevenue, orderCount, avg);
    }

    /// <summary>
    /// Returns top products by quantity sold (non-cancelled orders only).
    /// Each result: ProductId, ProductName (or "Product #id" if deleted), QuantitySold, Revenue.
    /// </summary>
    public List<(int ProductId, string ProductName, int QuantitySold, decimal Revenue)> GetTopProducts(int limit)
    {
        var validOrderIds = _db.Orders
            .Where(o => o.Status != OrderStatus.Cancelled)
            .Select(o => o.Id)
            .ToHashSet();

        var productTotals = _db.Orders
            .Where(o => validOrderIds.Contains(o.Id) && o.Items != null)
            .SelectMany(o => o.Items)
            .GroupBy(i => i.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                QuantitySold = g.Sum(i => i.Quantity),
                Revenue = g.Sum(i => i.Quantity * i.UnitPrice)
            })
            .OrderByDescending(x => x.QuantitySold)
            .Take(limit)
            .ToList();

        var productsById = _db.Products.ToDictionary(p => p.Id);

        return productTotals
            .Select(x => (
                x.ProductId,
                productsById.TryGetValue(x.ProductId, out var p) ? p.Name : $"Product #{x.ProductId}",
                x.QuantitySold,
                x.Revenue
            ))
            .ToList();
    }

    /// <summary>
    /// Returns revenue and order count grouped by period. Period is "day", "week", or "month".
    /// </summary>
    public List<(string PeriodLabel, decimal Revenue, int OrderCount)> GetRevenueByPeriod(string period)
    {
        var orders = _db.Orders
            .Where(o => o.Status != OrderStatus.Cancelled)
            .ToList();

        if (orders.Count == 0)
            return new List<(string, decimal, int)>();

        var calendar = CultureInfo.CurrentCulture.Calendar;

        var grouped = orders
            .Select(o =>
            {
                string label;
                DateTime key;
                switch (period?.ToLowerInvariant())
                {
                    case "day":
                        key = o.CreatedAt.Date;
                        label = key.ToString("yyyy-MM-dd");
                        break;
                    case "week":
                        key = o.CreatedAt.Date;
                        var week = calendar.GetWeekOfYear(key, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                        label = $"{key.Year}-W{week:D2}";
                        key = key.AddDays(-(int)key.DayOfWeek + (int)DayOfWeek.Monday);
                        break;
                    case "month":
                        key = new DateTime(o.CreatedAt.Year, o.CreatedAt.Month, 1);
                        label = key.ToString("yyyy-MM");
                        break;
                    default:
                        key = o.CreatedAt.Date;
                        label = key.ToString("yyyy-MM-dd");
                        break;
                }
                return (Label: label, Key: key, Order: o);
            })
            .GroupBy(x => (x.Label, x.Key))
            .OrderBy(g => g.Key.Key)
            .Select(g => (g.Key.Label, Revenue: g.Sum(x => x.Order.TotalAmount), OrderCount: g.Count()))
            .ToList();

        return grouped;
    }
}
