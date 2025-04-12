using System.Collections.Generic;

/// <summary>
/// 02-03-2025  1.0     Keelin      Create Model
/// </summary>
/// 
namespace CoralBleachingExtrapolation.ViewModels
{
    public class WorldCoralViewModel
    {
        public int GlobalCoralId { get; set; }
        public string? CoralName { get; set; }
        public List<CoordinateViewModel> Coordinates { get; set; } = new List<CoordinateViewModel>();
    }

    public class CoordinateViewModel
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}
