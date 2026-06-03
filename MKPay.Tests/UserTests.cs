using MKPay.Core.Entities;

namespace MKPay.Tests;

[TestFixture]
public class UserTests
{
    [Test]
    public void NewApplicationUser_IsActiveByDefault()
    {
        var user = new ApplicationUser();

        Assert.That(user.IsActive, Is.True);
    }

    [Test]
    public void NewApplicationUser_FirstName_IsEmptyStringByDefault()
    {
        var user = new ApplicationUser();

        Assert.That(user.FirstName, Is.EqualTo(string.Empty));
    }

    [Test]
    public void NewApplicationUser_ProfilePictureUrl_IsNullByDefault()
    {
        var user = new ApplicationUser();

        Assert.That(user.ProfilePictureUrl, Is.Null);
    }

    [Test]
    public void NewApplicationUser_SentTransactions_IsEmptyCollectionByDefault()
    {
        var user = new ApplicationUser();

        Assert.That(user.SentTransactions, Is.Empty);
    }

    [Test]
    public void NewApplicationUser_CreatedAt_IsSetToApproximatelyUtcNow()
    {
        var before = DateTime.UtcNow.AddSeconds(-1);

        var user = new ApplicationUser();

        var after = DateTime.UtcNow.AddSeconds(1);
        Assert.That(user.CreatedAt, Is.InRange(before, after));
    }
}
