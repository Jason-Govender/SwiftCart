using SwiftCart.Application.Interfaces;
using SwiftCart.Domain.Entities;
using SwiftCart.Infrastructure.Data;

namespace SwiftCart.Infrastructure.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly AppDb _db;

    public PaymentRepository(AppDb db) => _db = db;

    public void Add(Payment payment) => _db.Payments.Add(payment);

    public int GetNextId() =>
        _db.Payments.Count > 0 ? _db.Payments.Max(p => p.Id) + 1 : 1;
}
