﻿using System;
using System.Text;
using TrailSimulation.Core;

namespace TrailSimulation.Game
{
    /// <summary>
    ///     Used when the player attempts to go from travel command menu to continue on the trail after they have already been
    ///     told the vehicle is stuck and unable to move. This message purpose is to remind them what they need to do and why
    ///     they cannot continue without using the same message that the drive form triggers since it is intended to be spawned
    ///     when the problem occurs.
    /// </summary>
    public sealed class VehicleStuck : InputForm<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public VehicleStuck(IWindow window) : base(window)
        {
        }

        /// <summary>
        ///     Fired when dialog prompt is attached to active game Windows and would like to have a string returned.
        /// </summary>
        protected override string OnDialogPrompt()
        {
            var stuckPrompt = new StringBuilder();
            stuckPrompt.AppendLine($"{Environment.NewLine}You must trade for an ox");
            stuckPrompt.AppendLine($"to be able to continue.{Environment.NewLine}");
            return stuckPrompt.ToString();
        }

        /// <summary>
        ///     Fired when the dialog receives favorable input and determines a response based on this. From this method it is
        ///     common to attach another state, or remove the current state based on the response.
        /// </summary>
        /// <param name="reponse">The response the dialog parsed from simulation input buffer.</param>
        protected override void OnDialogResponse(DialogResponse reponse)
        {
            ClearForm();
        }
    }
}