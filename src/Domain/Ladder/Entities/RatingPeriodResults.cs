using System.Collections.Generic;

namespace Warhammer.Domain.Ladder.Entities
{
    /// <summary>
    /// This class holds the results accumulated over a rating period.
    /// </summary>
    public class RatingPeriodResults
    {
        private readonly List<Result> _results = new();
        private readonly HashSet<Player> _participants = new();

        /// <summary>
        /// Create an empty result set.
        /// </summary>
        public RatingPeriodResults()
        {
        }

        /// <summary>
        /// Constructor that allows you to initialise the list of participants.
        /// </summary>
        /// <param name="participants"></param>
        public RatingPeriodResults(HashSet<Player> participants)
        {
            this._participants = participants;
        }

        /// <summary>
        /// Add a result to the set.
        /// </summary>
        /// <param name="winner"></param>
        /// <param name="loser"></param>
        public void AddResult(Result result)
        {
	        this._results.Add(result);
        }

        /// <summary>
        /// Get a list of the results for a given player.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public IList<Result> GetResults(Player player)
        {
            var filteredResults = new List<Result>();

            foreach (var result in this._results)
            {
                if (result.Participated(player))
                {
                    filteredResults.Add(result);
                }
            }

            return filteredResults;
        }

        /// <summary>
        /// Get all the participants whose results are being tracked.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Player> GetParticipants()
        {
            // Run through the results and make sure all players have been pushed into the participants set.
            foreach (var result in this._results)
            {
                this._participants.Add(result.GetWinner());
                this._participants.Add(result.GetLoser());
            }

            return this._participants;
        }

        /// <summary>
        /// Add a participant to the rating period, e.g. so that their rating will
        /// still be calculated even if they don't actually compete.
        /// </summary>
        /// <param name="rating"></param>
        public void AddParticipant(Player rating)
        {
            this._participants.Add(rating);
        }

        /// <summary>
        /// Clear the result set.
        /// </summary>
        public void Clear()
        {
            this._results.Clear();
        }
    }
}