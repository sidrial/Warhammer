using System;
using Warhammer.Domain.Tournaments.Entities;
using Xunit;

namespace Warhammer.Domain.UnitTests.Tournaments;

public class MatchTests
{
    [Fact]
    public void Create_InvalidMatch_ShouldThrowException()
    {
	    //arrange
        var playerA = new PlayerBuilder().WithId("1").WithName("A").Build();
        var playerB = new PlayerBuilder().WithId("1").WithName("A").Build();

        //act
        Action constructInvalidMatch = () =>
        {
            new Match(new[] { playerA, playerB }, "1", "1");
        };

        //assert
        Assert.Throws<ArgumentException>(constructInvalidMatch);
    }

    [Fact]
    public void Create_ValidMatch_ShouldNotThrowException()
    {
        //arrange
        var playerA = new PlayerBuilder().WithName("A").Build();
        var playerB = new PlayerBuilder().WithName("B").Build();

        //act
        var match = new Match(new[] { playerA, playerB }, "1", "1");

        //assert
        Assert.NotNull(match);
    }
}