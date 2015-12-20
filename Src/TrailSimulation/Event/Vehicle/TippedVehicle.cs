﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     Vehicle was going around a bend, hit a bump, rough trail, or any of the following it now tipped over and supplies
    ///     could be destroyed and passengers can be crushed to death.
    /// </summary>
    [DirectorEvent(EventCategory.Vehicle)]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public sealed class TippedVehicle : EventItemDestroyer
    {
        /// <summary>
        ///     Fired by the item destroyer event prefab before items are destroyed.
        /// </summary>
        /// <param name="destroyedItems">Items that were destroyed from the players inventory.</param>
        protected override string OnPostDestroyItems(IDictionary<Entities, int> destroyedItems)
        {
            // Change event text depending on if items were destroyed or not.
            return destroyedItems.Count > 0
                ? TryKillPassengers("crushed")
                : "no loss of items.";
        }

        /// <summary>
        ///     Fired by the item destroyer event prefab after items are destroyed.
        /// </summary>
        protected override string OnPreDestroyItems()
        {
            var capsizePrompt = new StringBuilder();
            capsizePrompt.Clear();
            capsizePrompt.AppendLine("vehicle has tipped");
            capsizePrompt.Append("over resulting in ");
            return capsizePrompt.ToString();
        }
    }
}