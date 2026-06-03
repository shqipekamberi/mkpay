using MKPay.Core.Entities;
using MKPay.Core.Enums;

namespace MKPay.Tests;

[TestFixture]
public class TransactionTests
{
    [Test]
    public void NewTransaction_HasPendingStatusByDefault()
    {
        var transaction = new Transaction();

        Assert.That(transaction.Status, Is.EqualTo(TransactionStatus.Pending));
    }

    [Test]
    public void NewTransaction_BillSplitId_IsNullByDefault()
    {
        var transaction = new Transaction();

        Assert.That(transaction.BillSplitId, Is.Null);
    }

    [Test]
    public void NewTransaction_PaymentRequestId_IsNullByDefault()
    {
        var transaction = new Transaction();

        Assert.That(transaction.PaymentRequestId, Is.Null);
    }

    [Test]
    public void Transaction_StatusCanBeUpdatedToCompleted()
    {
        var transaction = new Transaction();

        transaction.Status = TransactionStatus.Completed;

        Assert.That(transaction.Status, Is.EqualTo(TransactionStatus.Completed));
    }

    [Test]
    public void Transaction_SenderAndReceiverAccountIds_CanBeSetIndependently()
    {
        var senderId = Guid.NewGuid();
        var receiverId = Guid.NewGuid();

        var transaction = new Transaction
        {
            SenderAccountId = senderId,
            ReceiverAccountId = receiverId
        };

        Assert.That(transaction.SenderAccountId, Is.EqualTo(senderId));
        Assert.That(transaction.ReceiverAccountId, Is.EqualTo(receiverId));
        Assert.That(transaction.SenderAccountId, Is.Not.EqualTo(transaction.ReceiverAccountId));
    }
}
