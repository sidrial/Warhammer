using System;

namespace Warhammer.Domain.Ladder.Entities
{
    /// <summary>
    /// Represents the result of a match between two players.
    /// </summary>
    public class Result
    {
        private const double PointsForWin = 1.0;
        private const double PointsForLoss = 0.0;
        private const double PointsForDraw = 0.5;

        private readonly bool _isDraw;
        private readonly Player _winner;
        private readonly Player _loser;

        /// <summary>
        /// Record a new result from a match between two players.
        /// </summary>
        /// <param name="winner"></param>
        /// <param name="loser"></param>
        /// <param name="isDraw"></param>
        public Result(Player winner, Player loser, bool isDraw = false)
        {
            if (!ValidPlayers(winner, loser))
            {
                throw new ArgumentException("Players winner and loser are the same player");
            }

            this._winner = winner;
            this._loser = loser;
            this._isDraw = isDraw;
        }

        /// <summary>
        /// Check that we're not doing anything silly like recording a match with only one player.
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        /// <returns></returns>
        private static bool ValidPlayers(Player player1, Player player2)
        {
            return player1 != player2;
        }

        /// <summary>
        /// Test whether a particular player participated in the match represented by this result.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool Participated(Player player)
        {
            return player == this._winner || player == this._loser;
        }

        /// <summary>
        /// Returns the "score" for a match.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public double GetScore(Player player)
        {
            double score;

            if (this._winner == player)
            {
                score = PointsForWin;
            }
            else if (this._loser == player)
            {
                score = PointsForLoss;
            }
            else
            {
                throw new ArgumentException("Player did not participate in match", "player");
            }

            if (this._isDraw)
            {
                score = PointsForDraw;
            }

            return score;
        }

        /// <summary>
        /// Given a particular player, returns the opponent.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public Player GetOpponent(Player player)
        {
            Player opponent;

            if (this._winner == player)
            {
                opponent = this._loser;
            }
            else if (this._loser == player)
            {
                opponent = this._winner;
            }
            else
            {
                throw new ArgumentException("Player did not participate in match", "player");
            }

            return opponent;
        }

        public Player GetWinner()
        {
            return this._winner;
        }

        public Player GetLoser()
        {
            return this._loser;
        }
    }
}