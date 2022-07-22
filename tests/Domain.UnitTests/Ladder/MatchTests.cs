namespace Warhammer.Domain.UnitTests.Ladder
{
    public class MatchTests
    {
	    //[Fact]
        public void GlickoUpdateCorrect()
        {
            // Instantiate a RatingCalculator object.
            // At instantiation, you can set the default rating for a player's volatility and
            // the system constant for your game ("τ", which constrains changes in volatility
            // over time) or just accept the defaults.
            //var calculator = new RatingCalculator();

            // Instantiate player.
            //var matchRepo = new MatchRepo();
            //var repo = new PlayerRepo();
            //var winner = repo.LoadByUserId(1).GetAwaiter().GetResult();
            //var loser = repo.LoadByUserId(2).GetAwaiter().GetResult();

            //var match = matchRepo.LoadById(3).GetAwaiter().GetResult();
            // Instantiate a RatingPeriodResults object.
            //var results = new RatingPeriodResults();

            // Add game results to the RatingPeriodResults object until you reach the end of your rating period.
            //var match = new Match(winner, 50, loser, 20, winner.UserId,false);
            //results.AddResult(match);

            // Use addParticipant(player) to add players that played no games in the rating period.
            //results.AddParticipant(player3);

            // Once you've reached the end of your rating period, call the updateRatings method
            // against the RatingCalculator; this takes the RatingPeriodResults object as argument.
            //  * Note that the RatingPeriodResults object is cleared down of game results once
            //    the new ratings have been calculated.
            //  * Participants remain within the RatingPeriodResults object, however, and will
            //    have their rating deviations recalculated at the end of future rating periods
            //    even if they don't play any games. This is in-line with Glickman's algorithm.
            //double currentRatingWinner = winner.Rating;

            //calculator.UpdateRatings(results);
            //var ratingDifference = winner.Rating - currentRatingWinner;
            //match.SetRatingChange(ratingDifference);


            //matchRepo.Insert(match).GetAwaiter().GetResult();
            //repo.UpdateMany(new List<Player> { winner, loser }).GetAwaiter().GetResult();

        }



    }
}