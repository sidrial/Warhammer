using Architect.Identities;
using Newtonsoft.Json;

namespace Warhammer.Domain.Ladder.Entities
{
    /// <summary>
    /// Holds an individual's Glicko-2 rating.
    /// 
    /// Glicko-2 ratings are an average skill value, a standard deviation and a volatility
    /// (how consistent the player is). Prof Glickman's paper on the algorithm allows scaling
    /// of these values to be more directly comparable with existing rating systems such as
    /// Elo or USCF's derivation thereof. This implementation outputs ratings at this larger
    /// scale.
    /// </summary>
    public class Player
    {
	    [JsonProperty("Id")]
        public string Id { get; private set; }
        
        [JsonProperty("Name")]
        public string Name { get; private set; }

        [JsonProperty("Rating")]
        public double Rating { get; private set; }

        [JsonProperty("RatingDeviation")]
        public double RatingDeviation { get; private set; }

        [JsonProperty("Volatility")]
        public double Volatility { get; private set; }

        private RatingCalculator _ratingSystem => new();

        /// <summary>
        /// The number of results from which the rating has been calculated.
        /// </summary>
        private int _numberOfResults;

        // the following variables are used to hold values temporarily whilst running calculations
        private double _workingRating;
        private double _workingRatingDeviation;
        private double _workingVolatility;

        public Player()
        {
        }

		/// <summary>
		/// Constructor for new Player. Takes the rating, deviation, and volatility default values from the rating system.
		/// </summary>
		public Player(string name)
		{
			this.Id = DistributedId.CreateId().ToAlphanumeric();
			this.Name = name;
			this.Rating = this._ratingSystem.GetDefaultRating();
			this.RatingDeviation = this._ratingSystem.GetDefaultRatingDeviation();
			this.Volatility = this._ratingSystem.GetDefaultVolatility();
        }
        
		public void SetPlayerName(string playerName)
		{
			this.Name = playerName;
		}

		public void UpdatePlayerRatings(double rating, double ratingDeviation, double volatility)
		{
			this.Rating = rating;
			this.RatingDeviation = ratingDeviation;
			this.Volatility = volatility;
        }

        /// <summary>
        /// Return the average skill value of the player scaled down
        /// to the scale used by the algorithm's internal workings.
        /// </summary>
        /// <returns></returns>
        public double GetGlicko2Rating()
        {
            return this._ratingSystem.ConvertRatingToGlicko2Scale(this.Rating);
        }

        /// <summary>
        /// Set the average skill value, taking in a value in Glicko2 scale.
        /// </summary>
        /// <param name="rating"></param>
        public void SetGlicko2Rating(double rating)
        {
	        this.Rating = this._ratingSystem.ConvertRatingToOriginalGlickoScale(rating);
        }

        public double GetVolatility()
        {
            return this.Volatility;
        }

        public void SetVolatility(double volatility)
        {
	        this.Volatility = volatility;
        }
        
        /// <summary>
        /// Return the rating deviation of the player scaled down
	    /// to the scale used by the algorithm's internal workings.
        /// </summary>
        /// <returns></returns>
        public double GetGlicko2RatingDeviation()
        {
            return this._ratingSystem.ConvertRatingDeviationToGlicko2Scale(this.RatingDeviation);
        }

        /// <summary>
        /// Set the rating deviation, taking in a value in Glicko2 scale.
        /// </summary>
        /// <param name="ratingDeviation"></param>
        public void SetGlicko2RatingDeviation(double ratingDeviation)
        {
	        this.RatingDeviation = this._ratingSystem.ConvertRatingDeviationToOriginalGlickoScale(ratingDeviation);
        }

        /// <summary>
        /// Used by the calculation engine, to move interim calculations into their "proper" places.
        /// </summary>
        public void FinaliseRating()
        {
            this.SetGlicko2Rating(this._workingRating);
            this.SetGlicko2RatingDeviation(this._workingRatingDeviation);
            this.SetVolatility(this._workingVolatility);

            this.SetWorkingRatingDeviation(0);
            this.SetWorkingRating(0);
            this.SetWorkingVolatility(0);
        }

        public void IncrementNumberOfResults(int increment)
        {
            this._numberOfResults = this._numberOfResults + increment;
        }

        public void SetWorkingVolatility(double workingVolatility)
        {
            this._workingVolatility = workingVolatility;
        }

        public void SetWorkingRating(double workingRating)
        {
            this._workingRating = workingRating;
        }

        public void SetWorkingRatingDeviation(double workingRatingDeviation)
        {
            this._workingRatingDeviation = workingRatingDeviation;
        }
    }

    public enum PlayerStatus
    {
        Pending = 0,
        Approved = 1,
        Declined = 2
    }
}