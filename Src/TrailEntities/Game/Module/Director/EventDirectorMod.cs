﻿using System;
using TrailEntities.Entity;
using TrailEntities.Event;
using TrailEntities.Simulation;

namespace TrailEntities.Game
{
    /// <summary>
    ///     Numbers events and allows them to propagate through it and to other parts of the simulation. Lives inside of the
    ///     game simulation normally.
    /// </summary>
    public sealed class EventDirectorMod
    {
        /// <summary>
        ///     Fired when an event has been triggered by the director.
        /// </summary>
        public delegate void EventTriggered(IEntity simEntity, DirectorEvent directorEvent);

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.Game.EventDirectorMod" /> class.
        /// </summary>
        public EventDirectorMod()
        {
            // Creates a new event factory, and event history list. 
            EventFactory = new EventFactory();
        }

        /// <summary>
        ///     Creates event items on behalf of the director when he rolls the dice looking for one to trigger.
        /// </summary>
        private EventFactory EventFactory { get; }

        /// <summary>
        ///     Fired when an event has been triggered by the director.
        /// </summary>
        public event EventTriggered OnEventTriggered;

        /// <summary>
        ///     Gathers all of the events by specified type and then rolls the virtual dice to determine if any of the events in
        ///     the enumeration should trigger.
        /// </summary>
        /// <param name="sourceEntity">Entity which will be affected by event if triggered.</param>
        /// <param name="eventType">Event type the dice will be rolled against and attempted to trigger.</param>
        public void TriggerEventByType(IEntity sourceEntity, EventType eventType)
        {
            // Roll the dice here to determine if the event is triggered at all.
            var diceRoll = GameSimulationApp.Instance.Random.Next(100);
            if (diceRoll > 0)
                return;

            // Create a random event by type enumeration, event factory will randomly pick one for us based on the enum value.
            var randomEventProduct = EventFactory.CreateRandomByType(eventType);
            ExecuteEvent(sourceEntity, randomEventProduct);
        }

        /// <summary>
        ///     Triggers an event directly by type of reference. Event must have [EventDirectorMod] attribute to be registered in the
        ///     factory correctly.
        /// </summary>
        /// <param name="sourceEntity">Entity which will be affected by event if triggered.</param>
        /// <param name="eventType">System type that represents the type of event to trigger.</param>
        public void TriggerEvent(IEntity sourceEntity, Type eventType)
        {
            // Grab the event item from the factory that makes them.
            var eventProduct = EventFactory.CreateInstance(eventType);
            ExecuteEvent(sourceEntity, eventProduct);
        }

        /// <summary>
        ///     Primary worker for the event factory, pulled into it's own method here so all the trigger event types can call it.
        ///     This will attach the random event game mode and then fire an event to trigger the event execution in that mode
        ///     then it will be able to display any relevant data about what happened.
        /// </summary>
        /// <param name="sourceEntity">Entity which will be affected by event if triggered.</param>
        /// <param name="directorEvent">Created instance of event that will be executed on simulation in random game mode.</param>
        private void ExecuteEvent(IEntity sourceEntity, DirectorEvent directorEvent)
        {
            // Attach random event game mode before triggering event since it will listen for it using event delegate.
            GameSimulationApp.Instance.WindowManager.AddMode(GameMode.RandomEvent);

            // Fire off event so primary game simulation knows we executed an event with an event.
            OnEventTriggered?.Invoke(sourceEntity, directorEvent);
        }
    }
}