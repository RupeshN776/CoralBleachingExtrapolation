using Microsoft.AspNetCore.Mvc;

namespace CoralBleachingExtrapolation.ViewModels
{
    public class GBRCoralViewModel : Controller
    {
        public int GlobalCoralId { get; set; }
        public string? ReefName { get; set; }
        public List<CoordinateViewModelGBR> Coordinates { get; set; } = new List<CoordinateViewModelGBR>();
    }

    public class CoordinateViewModelGBR
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}