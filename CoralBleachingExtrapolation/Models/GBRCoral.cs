using Azure.Core.GeoJson;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;
using NetTopologySuite.Algorithm;
using CoralBleachingExtrapolation.Data;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 03-02-2025  1.0     Keelin   Point GBR Implimenting UML (GBRCoral.cs) 
/// 03-02-2025  1.0.1   Keelin   Fixing references and errors (InvalidOperationException, InvalidColumnName) 
/// 03-22-2025  1.0.2   Keelin   Making it work with the index 
/// 03-24-2025  1.1     Keelin   Adjusted to new Database changes. 
/// </summary>
/// 

namespace CoralBleachingExtrapolation.Models
{
    //removed. 

    //public class MedianLiveCoral
    //{
    //    [Key]
    //    public int MedianLiveCoral_ID { get; set; }
    //    public string? Median { get; set; }
    //}


    //public class MedianDeadCoral
    //{
    //    [Key]
    //    public int MedianDeadCoral_ID { get; set; }
    //    public string? Median { get; set; }

    //}


    //public class MedianSoftCoral
    //{
    //    [Key]
    //    public int MedianSoftCoral_ID { get; set; }
    //    public string? Median { get; set; }
    //}

    public class GBRCoralPoint
    {
        [Key]
        public int GBRCoralPointID { get; set; }
        public string? ReefName { get; set; }
        public Point? Point { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int? ReportYear { get; set; }
        public double? MeanLiveCoral { get; set; }
        public double? MeanSoftCoral { get; set; }
        public double? MeanDeadCoral { get; set; }
        public int?  MedianLiveCoralFK { get; set; }
        public int? MedianSoftCoralFK { get; set; }
        public  int? MedianDeadCoralFK { get; set; }
    }

    //P: okay

}
