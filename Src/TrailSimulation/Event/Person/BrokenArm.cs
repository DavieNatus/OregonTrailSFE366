﻿using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     One of the members of the vehicle passenger manifest broke their arm somehow.
    /// </summary>
    [DirectorEvent(EventCategory.Person)]
    public sealed class BrokenArm : EventPersonInjure
    {
        /// <summary>
        ///     Fired after the event has executed and the injury flag set on the person.
        /// </summary>
        /// <param name="person">Person whom is now injured by whatever you say they are here.</param>
        /// <returns>Describes what type of physical injury has come to the person.</returns>
        protected override string OnPostInjury(Person person)
        {
            return $"{person.Name} has broken their arm.";
        }
    }
}