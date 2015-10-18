﻿using System;
using TrailCommon;

namespace TrailEntities
{
    /// <summary>
    ///     Used to allow the players party to hunt for wild animals, shooting bullet items into the animals will successfully
    ///     kill them and when the round is over the amount of meat is determined by what animals are killed. The player party
    ///     can only take back up to one hundred pounds of whatever the value was back to the wagon regardless of what it was.
    /// </summary>
    public sealed class HuntMode : GameMode, IHunt
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:TrailEntities.GameMode" /> class.
        /// </summary>
        public HuntMode(IGameSimulation game) : base(game)
        {
        }

        public override ModeType Mode
        {
            get { return ModeType.Hunt; }
        }

        public void UseBullets(uint amount)
        {
            throw new NotImplementedException();
        }

        public void AddFood(uint amount)
        {
            throw new NotImplementedException();
        }

        public void UpdateVehicle()
        {
            throw new NotImplementedException();
        }
    }
}