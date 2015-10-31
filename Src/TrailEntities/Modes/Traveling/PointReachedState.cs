﻿using System.Text;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     State that is attached when the event is fired for reaching a new point of interest on the trail. Default action is
    ///     to ask the player if they would like to look around, but there is a chance for this behavior to be overridden in
    ///     the constructor there is a default boolean value to skip the question asking part and force a look around event to
    ///     occur without player consent.
    /// </summary>
    public sealed class PointReachedState : ModeState<TravelInfo>
    {
        /// <summary>
        ///     This constructor will be used by the other one
        /// </summary>
        public PointReachedState(IMode gameMode, TravelInfo userData, bool forceLookAround = false)
            : base(gameMode, userData)
        {
            UserData.ForceLookAround = forceLookAround;

            // Skip asking the player anything and go right to next game mode state.
            if (UserData.ForceLookAround)
            {
                ParentMode.CurrentState = new LookAroundState(ParentMode, UserData);
            }
        }

        /// <summary>
        ///     Returns a text only representation of the current game mode state. Could be a statement, information, question
        ///     waiting input, etc.
        /// </summary>
        public override string GetStateTUI()
        {
            // Wait for input on deciding if we should take a look around.
            var pointReached = new StringBuilder();
            pointReached.Append($"You are now at the {ParentMode.CurrentPoint.Name}.\n");
            pointReached.Append("Would you like to look around? Y/N");
            return pointReached.ToString();
        }

        /// <summary>
        ///     Fired when the game mode current state is not null and input buffer does not match any known command.
        /// </summary>
        /// <param name="input">Contents of the input buffer which didn't match any known command in parent game mode.</param>
        public override void OnInputBufferReturned(string input)
        {
            // Do not accept input if we are forced to look around.
            if (UserData.ForceLookAround)
                return;

            // If use wants to look around attach that mode, other wise just remove current state and go back to travel mode.
            switch (input.ToUpperInvariant())
            {
                case "Y":
                    ParentMode.CurrentState = new LookAroundState(ParentMode, UserData);
                    break;
                default:
                    ParentMode.CurrentState = null;
                    break;
            }
        }
    }
}