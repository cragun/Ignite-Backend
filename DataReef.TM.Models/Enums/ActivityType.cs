using System;

namespace DataReef.TM.Models
{
    [Flags]
    public enum ActivityType
    {
        None = 0,
        SolarPanels = 1 << 0,
        TV = 1 << 1,
        Survey = 1 << 2,
        HomeSystems = 1 << 3
    }
}