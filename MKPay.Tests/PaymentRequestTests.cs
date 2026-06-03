using MKPay.Core.Entities;
using MKPay.Core.Enums;

namespace MKPay.Tests;

[TestFixture]
public class PaymentRequestTests
{
    [Test]
    public void NewPaymentRequest_HasPendingStatusByDefault()
    {
        var request = new PaymentRequest();

        Assert.That(request.Status, Is.EqualTo(PaymentRequestStatus.Pending));
    }

    [Test]
    public void NewPaymentRequest_RespondedAt_IsNullByDefault()
    {
        var request = new PaymentRequest();

        Assert.That(request.RespondedAt, Is.Null);
    }

    [Test]
    public void NewPaymentRequest_Transactions_IsEmptyCollectionByDefault()
    {
        var request = new PaymentRequest();

        Assert.That(request.Transactions, Is.Empty);
    }

    [Test]
    public void PaymentRequest_IsExpired_WhenExpiresAtIsInThePast()
    {
        var request = new PaymentRequest
        {
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        };

        var isExpired = request.ExpiresAt < DateTime.UtcNow;

        Assert.That(isExpired, Is.True);
    }

    [Test]
    public void PaymentRequest_StatusCanBeUpdatedToAccepted()
    {
        var request = new PaymentRequest();

        request.Status = PaymentRequestStatus.Accepted;
        request.RespondedAt = DateTime.UtcNow;

        Assert.That(request.Status, Is.EqualTo(PaymentRequestStatus.Accepted));
        Assert.That(request.RespondedAt, Is.Not.Null);
    }
}
