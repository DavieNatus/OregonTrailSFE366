﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Location.cs" company="Ron 'Maxwolf' McDowell">
//   ron.mcdowell@gmail.com
// </copyright>
// <summary>
//   Defines a location in the game that is added to a list of points that make up the entire trail which the player and
//   his vehicle travel upon.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace TrailSimulation.Entity
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using Game;

    /// <summary>
    ///     Defines a location in the game that is added to a list of points that make up the entire trail which the player and
    ///     his vehicle travel upon.
    /// </summary>
    public sealed class Location : IEntity
    {
        /// <summary>
        ///     List of possible alternate locations the player might have to choose from if this list is not null and greater than
        ///     one. Game simulation will ask using modes and states what choice player would like.
        /// </summary>
        private List<Location> _skipChoices;

        /// <summary>
        ///     References all of the possible trades this location will be able to offer the player. If the list is empty that
        ///     means nobody wants to trade with the player at this time.
        /// </summary>
        private List<SimItem> _trades;

        /// <summary>
        ///     Deals with the weather simulation for this location, each location on the trail is capable of simulating it's own
        ///     type of weather for the purposes of keeping them unique.
        /// </summary>
        private LocationWeather _weather;

        /// <summary>Initializes a new instance of the <see cref="T:TrailSimulation.Entity.Location"/> class.</summary>
        /// <param name="name">The name.</param>
        /// <param name="category">The category.</param>
        /// <param name="climateType">The climate Type.</param>
        /// <param name="skipChoices">The skip Choices.</param>
        /// <param name="riverOption">The river Option.</param>
        public Location(
            string name, 
            LocationCategory category, 
            Climate climateType, 
            IEnumerable<Location> skipChoices = null, 
            RiverOption riverOption = RiverOption.FloatAndFord)
        {
            // Determines if this location will have fresh water.
            FreshWater = GameSimulationApp.Instance.Random.NextBool();

            // Creates a new system to deal with the management of the weather for this given location.
            _weather = new LocationWeather(climateType);

            // Trades are randomly generated when ticking the location.
            _trades = new List<SimItem>();

            // Offers up a decision when traveling on the trail, there are normally one of many possible outcomes.
            if (skipChoices != null)
                _skipChoices = new List<Location>(skipChoices);

            // Throws an exception if the river option is set to none and not a usable option.
            if (riverOption == RiverOption.None && category == LocationCategory.RiverCrossing)
                throw new ArgumentException("Unable to create location with river option set to NONE!");

            // Set the river option into the location itself.
            RiverCrossOption = riverOption;

            // Name of the point as it should be known to the player.
            Name = name;

            // Default location status is not visited by the player or vehicle.
            Status = LocationStatus.Unreached;

            // Category of the location determines how the game simulation will treat it.
            Category = category;
            switch (category)
            {
                case LocationCategory.Settlement:
                    ShoppingAllowed = true;
                    ChattingAllowed = true;
                    break;
                case LocationCategory.Landmark:
                case LocationCategory.RiverCrossing:
                case LocationCategory.ForkInRoad:
                    ShoppingAllowed = false;
                    ChattingAllowed = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, null);
            }
        }

        /// <summary>
        ///     Defines the type of river crossing this location will be, this is in regards to the types of options presented when
        ///     the crossing comes up in the trail.
        /// </summary>
        public RiverOption RiverCrossOption { get; }

        /// <summary>
        ///     Locations have fresh water flag if enabled doubles chance for dysentery and cholera.
        /// </summary>
        private bool FreshWater { get; }

        /// <summary>
        ///     Current weather condition this location is experiencing.
        /// </summary>
        public Weather Weather
        {
            get { return _weather.Condition; }
        }

        /// <summary>
        ///     Determines if the location allows the player to chat to other NPC's in the area which can offer up advice about the
        ///     trail ahead.
        /// </summary>
        public bool ChattingAllowed { get; private set; }

        /// <summary>
        ///     Defines the type of location this is, the game simulation will trigger and load different states depending on this
        ///     value. Defaults to default value which is a normal location with nothing special happening.
        /// </summary>
        public LocationCategory Category { get; }

        /// <summary>
        ///     Determines if this location has a store which the player can buy items from using their monies.
        /// </summary>
        public bool ShoppingAllowed { get; }

        /// <summary>
        ///     Determines if this location has already been visited by the vehicle and party members.
        /// </summary>
        /// <returns>TRUE if location has been passed by, FALSE if location has yet to be reached.</returns>
        public LocationStatus Status { get; set; }

        /// <summary>
        ///     Defines all of the skip choices that were defined for this location. Will return null if there are no skip choices
        ///     associated with this location.
        /// </summary>
        public ReadOnlyCollection<Location> SkipChoices
        {
            get { return _skipChoices.AsReadOnly(); }
        }

        /// <summary>
        ///     References all of the possible trades this location will be able to offer the player. If the list is empty that
        ///     means nobody wants to trade with the player at this time.
        /// </summary>
        public ReadOnlyCollection<SimItem> Trades
        {
            get { return _trades.AsReadOnly(); }
        }

        /// <summary>
        ///     Determines if the look around question has been asked in regards to the player stopping the vehicle to rest or
        ///     change vehicle options. Otherwise they will just continue on the trail, this property prevents the question from
        ///     being asked twice for any one location.
        /// </summary>
        public bool ArrivalFlag { get; set; }

        /// <summary>
        ///     Determines if the given location is the last location on the trail, this is useful to know because we want to do
        ///     something special with the location before we actually arrive to it but we can know the next location is last using
        ///     this.
        /// </summary>
        public bool IsLast { get; set; }

        /// <summary>
        ///     Total distance to the next location the player must travel before it will be triggered
        /// </summary>
        public int TotalDistance { get; set; }

        /// <summary>
        ///     Name of the current point of interest as it should be known to the player.
        /// </summary>
        public string Name { get; }

        /// <summary>Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the
        ///     other.</summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in
        ///     the following table.Value Meaning Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than<paramref name="y"/>.</returns>
        public int Compare(IEntity x, IEntity y)
        {
            Debug.Assert(x != null, "x != null");
            Debug.Assert(y != null, "y != null");

            var result = string.Compare(x.Name, y.Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

        /// <summary>Compares the current object with another object of the same type.</summary>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has the following
        ///     meanings: Value Meaning Less than zero This object is less than the <paramref name="other"/> parameter.Zero This
        ///     object is equal to <paramref name="other"/>. Greater than zero This object is greater than<paramref name="other"/>.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public int CompareTo(IEntity other)
        {
            Debug.Assert(other != null, "other != null");

            var result = string.Compare(other.Name, Name, StringComparison.Ordinal);
            if (result != 0) return result;

            return result;
        }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <returns>true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.</returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(IEntity other)
        {
            // Reference equality check
            if (this == other)
            {
                return true;
            }

            if (other == null)
            {
                return false;
            }

            if (other.GetType() != GetType())
            {
                return false;
            }

            if (Name.Equals(other.Name))
            {
                return true;
            }

            return false;
        }

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        public bool Equals(IEntity x, IEntity y)
        {
            return x.Equals(y);
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <returns>A hash code for the specified object.</returns>
        /// <param name="obj">The <see cref="T:System.Object"/> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj"/> is a reference type and<paramref name="obj"/> is null.</exception>
        public int GetHashCode(IEntity obj)
        {
            var hash = 23;
            hash = (hash*31) + Name.GetHashCode();
            return hash;
        }

        /// <summary>Called when the simulation is ticked by underlying operating system, game engine, or potato. Each of these system
        ///     ticks is called at unpredictable rates, however if not a system tick that means the simulation has processed enough
        ///     of them to fire off event for fixed interval that is set in the core simulation by constant in milliseconds.</summary>
        /// <remarks>Default is one second or 1000ms.</remarks>
        /// <param name="systemTick">TRUE if ticked unpredictably by underlying operating system, game engine, or potato. FALSE if
        ///     pulsed by game simulation at fixed interval.</param>
        /// <param name="skipDay">Determines if the simulation has force ticked without advancing time or down the trail. Used by
        ///     special events that want to simulate passage of time without actually any actual time moving by.</param>
        public void OnTick(bool systemTick, bool skipDay)
        {
            // Skip system ticks.
            if (systemTick)
                return;

            // Weather will only be ticked when not skipping a day.
            if (!skipDay)
                _weather.Tick();

            // TODO: Trades are randomly generated when ticking the location every day.
        }
    }
}