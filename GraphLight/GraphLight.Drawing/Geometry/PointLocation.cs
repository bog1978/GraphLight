using System;

namespace GraphLight.Geometry
{
    [Flags]
    public enum PointLocation
    {
        Default = 0,
        Left = 1,
        Right = 2,
        Behind = 4,
        Beyond = 8,
        Origin = 16,
        Destination = 32,
        Between = 64
    }
}