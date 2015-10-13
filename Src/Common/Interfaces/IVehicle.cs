﻿using System.Collections.ObjectModel;

namespace OregonTrail
{
    /// <summary>
    ///     Defines a basic vessel to carry parties of people. It's presence is defined by a current location, the amount of
    ///     distance it has traveled, and the parts that make up the entire vehicle (each with it's own health).
    /// </summary>
    public interface IVehicle : IEntity
    {
        uint DistanceTraveled { get; }
        ReadOnlyCollection<Item> Parts { get; }
    }
}