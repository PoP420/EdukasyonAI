using EdukasyonAI.Domain.Entities;
using EdukasyonAI.Domain.Shared.Enums;
using Xunit;

namespace EdukasyonAI.Domain.Tests;

public class ApplicationUserTests
{
    [Theory]
    [InlineData(12, true)]
    [InlineData(10, true)]
    [InlineData(18, false)]
    public void IsUnder13_ReturnsCorrectValue(int ageYears, bool expectedUnder13)
    {
        // Subtract an extra day so the age calculation is definitively above the threshold
        var user = new ApplicationUser
        {
            DateOfBirth = DateTime.UtcNow.AddYears(-ageYears).AddDays(-1)
        };

        Assert.Equal(expectedUnder13, user.IsUnder13);
    }

    [Fact]
    public void IsUnder13_ExactlyThirteen_ReturnsFalse()
    {
        // Born at least 13 full years + 2 days ago — definitively 13+
        var user = new ApplicationUser
        {
            DateOfBirth = DateTime.UtcNow.AddYears(-13).AddDays(-2)
        };
        Assert.False(user.IsUnder13);
    }

    [Fact]
    public void IsUnder13_NullDateOfBirth_ReturnsFalse()
    {
        var user = new ApplicationUser { DateOfBirth = null };
        Assert.False(user.IsUnder13);
    }
}
