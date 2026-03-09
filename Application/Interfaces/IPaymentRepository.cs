using SwiftCart.Domain.Entities;

namespace SwiftCart.Application.Interfaces;

public interface IPaymentRepository
{
    void Add(Payment payment);
    int GetNextId();
}
