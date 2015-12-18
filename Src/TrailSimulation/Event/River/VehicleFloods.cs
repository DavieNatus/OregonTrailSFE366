﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using TrailSimulation.Entity;
using TrailSimulation.Game;

namespace TrailSimulation.Event
{
    /// <summary>
    ///     When crossing a river there is a chance that your wagon will flood if you choose to caulk and float across the
    ///     river.
    /// </summary>
    [DirectorEvent(EventCategory.RiverCross, EventExecution.ManualOnly)]
    public sealed class VehicleFloods : EventItemDestroyer
    {
        /// <summary>
        ///     Fired by the item destroyer event prefab before items are destroyed.
        /// </summary>
        /// <param name="destroyedItems">Items that were destroyed from the players inventory.</param>
        protected override string OnPostDestroyItems(IDictionary<Entities, int> destroyedItems)
        {
            // Change event text depending on if items were destroyed or not.
            var postDestroy = new StringBuilder();
            if (destroyedItems.Count > 0)
            {
                postDestroy.AppendLine("the loss of:");

                // Attempts to kill the living passengers of the vehicle.
                var drownedPassengers = GameSimulationApp.Instance.Vehicle.Passengers.TryKill();

                // If the killed passenger list contains any entries we print them out.
                foreach (var person in drownedPassengers)
                {
                    // Only proceed if person is actually dead.
                    if (person.HealthValue == HealthLevel.Dead)
                        postDestroy.AppendLine($"{person.Name} (drowned)");
                }
            }
            else
            {
                // Player got lucky and nothing destroyed and nobody killed.
                postDestroy.AppendLine("no loss of items.");
            }

            // Returns the processed flooding event for rendering.
            return postDestroy.ToString();
        }

        /// <summary>
        ///     Fired when the event handler associated with this enum type triggers action on target entity. Implementation is
        ///     left completely up to handler.
        /// </summary>
        /// <param name="sourceEntity">
        ///     Entities which the event is going to directly affect. This way there is no confusion about
        ///     what entity the event is for. Will require casting to correct instance type from interface instance.
        /// </param>
        public override void Execute(IEntity sourceEntity)
        {
            base.Execute(sourceEntity);

            // Cast the source entity as vehicle.
            var vehicle = sourceEntity as Vehicle;
            Debug.Assert(vehicle != null, "vehicle != null");

            // Reduce the total possible mileage of the vehicle this turn.
            vehicle.ReduceMileage(20 - 20*GameSimulationApp.Instance.Random.Next());
        }

        /// <summary>
        ///     Fired by the item destroyer event prefab after items are destroyed.
        /// </summary>
        protected override string OnPreDestroyItems()
        {
            var _floodPrompt = new StringBuilder();
            _floodPrompt.Clear();
            _floodPrompt.AppendLine("Vehicle floods");
            _floodPrompt.AppendLine("while crossing the");
            _floodPrompt.Append("river results in");
            return _floodPrompt.ToString();
        }
    }
}