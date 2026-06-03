using MKPay.Core.Entities;
using MKPay.Core.Enums;

namespace MKPay.Tests;

[TestFixture]
public class AccountServiceTests
{
    [Test]
    public void NewAccount_HasDefaultBalanceOfZero()
    {
        var account = new Account();

        Assert.That(account.Balance, Is.EqualTo(0));
    }

    [Test]
    public void NewAccount_HasMKDCurrencyByDefault()
    {
        var account = new Account();

        Assert.That(account.Currency, Is.EqualTo(Currency.MKD));
    }

    [Test]
    public void NewAccount_IsActiveByDefault()
    {
        var account = new Account();

        Assert.That(account.IsActive, Is.True);
    }

    [Test]
    public void NewAccount_HasNonEmptyGeneratedId()
    {
        var account = new Account();

        Assert.That(account.Id, Is.Not.EqualTo(Guid.Empty));
    }

    [Test]
    public void Account_WhenDeactivated_IsActiveIsFalse()
    {
        var account = new Account { IsActive = false };

        Assert.That(account.IsActive, Is.False);
    }
}
