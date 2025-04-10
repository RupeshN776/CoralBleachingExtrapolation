namespace CoralBleachingExtrapolation.Models
{
    public class CoralCreationViewModel

    ///Date        Version          Name        Comment
    /// 10-4-2025   1.0             Rupesh      Initial as i did not want to alter World.cs
    ///
    ///

    {
        public string CoralType { get; set; } //we have 10 types
        public string BasePolygonWKT { get; set; } // Shape in WKT 

        // Additional fields
        public string OriginName { get; set; }
        public string Family { get; set; }
        public string Genus { get; set; }
        public string Species { get; set; }
        public double? GISAREAKM2 { get; set; }
        public int? RegionFK { get; set; }

    }
}
