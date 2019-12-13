using System;

namespace DataReef.TM.Models.DTOs.Tables
{
    [Flags]
    public enum VisibleBorders
    {
        None = 0,
        Top = 1,
        Left = 2,
        Right = 4,
        Bottom = 8
    }
}
