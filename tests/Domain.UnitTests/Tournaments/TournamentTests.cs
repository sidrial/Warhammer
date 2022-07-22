using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Warhammer.Domain.Tournaments.Enums;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Warhammer.Domain.UnitTests.Tournaments;

public class TournamentTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TournamentTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }


    [Fact]
    public void Tournament_Progress_DoesNotThrow()
    {
        //arrange
        var tournament = new TournamentBuilder().Build();

        //act
        for (var i = 0; i < 50; i++)
        {
            tournament.Progress();
        }

        //assert
        // code above does not throw
    }

    [Fact]
    public void Tournament_CreateNextRound_IncreasesNumberOfRounds()
    {
        //arrange
        var tournament = new TournamentBuilder().Build();
        var currentNumberOfRounds = tournament.Rounds.Count;

        //act
        tournament.AddNextRound();

        //assert
        Assert.Equal(currentNumberOfRounds + 1, tournament.Rounds.Count);
    }

    [Fact]
    public void Tournament_CreateNextRound_DoesNotExceedMaxLength()
    {
        //arrange
        var tournament = new TournamentBuilder().WithLength(0).Build();

        //act + assert
        Assert.Throws<ValidationException>(() => tournament.AddNextRound());
    }

    [Fact]
    public void Tournament_GetPlayerScoreWinner_EqualsSumOfMatchPoints_PlusThousand()
    {
        //arrange
        var tournament = new TournamentBuilder().Build();
        tournament.Players.Add(new PlayerBuilder().WithId("PlayerAId").WithName("A").Build());
        tournament.Players.Add(new PlayerBuilder().Build());
        tournament.AddNextRound();

        // act
        var matchToUpdate = tournament.Rounds[0].Matches[0];
        uint playerOneScore = 30;
        uint playerTwoScore = 50;
        if (matchToUpdate.Player1.Id == "PlayerAId")
        {
            playerOneScore = 50;
            playerTwoScore = 30;
        }

        tournament.UpdateMatch(matchToUpdate.Id, playerOneScore, playerTwoScore, 1111, true);
        var playerAScore = tournament.GetPlayerScore("PlayerAId");

        // assert
        Assert.Equal(1050, (int)playerAScore);
    }

    [Fact]
    // A functional / integration test, but creating a new project for 1 test is overkill.
    public void Run_Full_Tournament()
    {
        //arrange
        var tournament = new TournamentBuilder().Build();
        tournament.Players.Add(new PlayerBuilder().WithName("A").Build());
        tournament.Players.Add(new PlayerBuilder().WithName("B").Build());
        tournament.Players.Add(new PlayerBuilder().WithName("C").Build());
        tournament.Players.Add(new PlayerBuilder().WithName("D").Build());
        tournament.Players.Add(new PlayerBuilder().WithName("E").Build());
        tournament.Players.Add(new PlayerBuilder().WithName("F").Build());
        tournament.Progress();
        tournament.Progress();

        for (int i = 0; i < tournament.Length; i++)
        {
            tournament.AddNextRound();

            foreach (var match in tournament.Rounds[i].Matches)
            {
                var random = new Random();
                var playerOneScore = random.Next(1, 99);
                var playerTwoScore = random.Next(1, 99);
                tournament.UpdateMatch(match.Id, (uint)playerOneScore, (uint)playerTwoScore, 1111, true);
            }
        }
        tournament.Progress();

        Assert.Equal(TournamentStatus.Finished, tournament.Status); // Tournament is finished
        Assert.Equal(tournament.Length, (uint)tournament.Rounds.Count); // Tournament ran for number of predefined rounds

        var matchesByPlayerGrouped = tournament.Rounds.SelectMany(round => round.Matches).SelectMany(match => match.Players).GroupBy(player => player.Name).ToList();
        Assert.True(matchesByPlayerGrouped.All(group => group.Count() == tournament.Length));
        
         var players = tournament.Players?.OrderByDescending(p => p.CalculatedPlayerScore).ToList();
        _testOutputHelper.WriteLine("Player final standings:");
        foreach (var player in players)
        {
            _testOutputHelper.WriteLine($"{player} - {player.CalculatedPlayerScore}");
        }

        _testOutputHelper.WriteLine("Match results:");
        var allMatches = tournament.Rounds.SelectMany(round => round.Matches);
        foreach (var match in allMatches)
        {
	        _testOutputHelper.WriteLine(match.ToString());
        }
    }
}