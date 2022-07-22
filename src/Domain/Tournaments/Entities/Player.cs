using System;
using Architect.Identities;
using Newtonsoft.Json;
using Warhammer.Domain.Tournaments.ValueObjects;

namespace Warhammer.Domain.Tournaments.Entities
{
    public class Player
    {
	    [JsonProperty("Id")]
        public string Id { get; private set; }
        
        [JsonProperty("TournamentId")]
        public string TournamentId { get; private set; }

        [JsonProperty("Name")]
        public string Name { get; private set; }

        [JsonProperty("Email")]
        public string Email { get; private set; }

        [JsonProperty("Club")]
        public string Club { get; private set; }

        [JsonProperty("Faction")]
        public string? Faction { get; private set; }

        [JsonProperty("List")]
        public string? List { get; private set; }

        [JsonProperty("Points")]
        public uint Points { get; set; }

        [JsonProperty("Secret")]
        public uint Pin { get; private set; }

        [JsonProperty("ExtraPointsListDeadline")]
        public bool ExtraPointsListDeadline { get; private set; }

        [JsonProperty("ExtraPointsListValid")]
        public bool ExtraPointsListValid { get; private set; }

        [JsonProperty("ExtraPointsListPainted")]
        public ushort ExtraPointsListPainted { get; private set; }
        
        public uint CalculatedPlayerScore {
            get
            {
                var score = this.Points;
                if (this.ExtraPointsListDeadline) score += 125;
                if (this.ExtraPointsListValid) score += 125;
                score += this.ExtraPointsListPainted;
                
                return score;
            }
        }
        
        public Player(string tournamentId, string name, string email, string club)
        {
	        this.Id = DistributedId.CreateId().ToAlphanumeric();
            this.TournamentId = tournamentId;
            this.Name = name;
            this.Email = email;
            this.Club = club;
            this.Points = 0;
            var rnd = new Random();
            this.Pin = new Pin((uint)rnd.Next(1000, 9999));
        }

        public void Update(string club, string faction, string list)
        {
            this.Club = club;
            this.Faction = faction;
            this.List = list;
        }

        public void SetExtraPoints(bool extraPointsListDeadline, bool extraPointsListValid, ushort extraPointsListPainted)
        {
            this.ExtraPointsListDeadline = extraPointsListDeadline;
            this.ExtraPointsListValid = extraPointsListValid;
            this.ExtraPointsListPainted = extraPointsListPainted;
        }

        public override string ToString() => this.Name;

        public override bool Equals(object other)
        {
            if (Object.ReferenceEquals(other, null))
                return false;

            if (other is Player)
                return this.Equals((Player)other);

            return false;
        }

        public bool Equals(Player other)
        {
            if (Object.ReferenceEquals(other, null))
                return false;

            if (Object.ReferenceEquals(this, other))
                return true;

            return this.Id == other.Id;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }

        public static bool operator ==(Player x, Player y)
        {
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return Object.ReferenceEquals(x, null) && Object.ReferenceEquals(y, null);

            return x.Equals(y);
        }

        public static bool operator !=(Player x, Player y)
        {
            return !(x == y);
        }
    }
}