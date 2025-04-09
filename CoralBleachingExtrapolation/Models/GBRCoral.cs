using Azure.Core.GeoJson;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;
using NetTopologySuite.Algorithm;
using CoralBleachingExtrapolation.Data;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// 03-02-2025  1.0     Keelin   Point GBR Implimenting UML (GBRCoral.cs) 
/// 03-02-2025  1.0.1   Keelin   Fixing references and errors (InvalidOperationException, InvalidColumnName) 
/// 03-22-2025  1.0.2   Keelin   Making it work with the index 
/// 03-24-2025  1.1     Ben      Json Serialize
/// </summary>
/// 

namespace CoralBleachingExtrapolation.Models
{
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

        [Required(ErrorMessage = "Latitude is required.")]
        public decimal? Latitude { get; set; }
        [Required(ErrorMessage = "Longitude is required.")]
        public decimal? Longitude { get; set; }
        public int? ReportYear { get; set; }
        public double? MeanLiveCoral { get; set; }
        public double? MeanSoftCoral { get; set; }
        public double? MeanDeadCoral { get; set; }
        public int? MedianLiveCoralFK { get; set; }
        public int? MedianSoftCoralFK { get; set; }
        public int? MedianDeadCoralFK { get; set; }

        public string ToJson()
        {
            return JsonSerializer.Serialize(new
            {
                Latitude = Latitude.HasValue ? (float)Latitude.Value : (float?)null,
                Longitude = Longitude.HasValue ? (float)Longitude.Value : (float?)null,
                ReefName,
                ReportYear,
                MeanLiveCoral,
                MeanSoftCoral,
                MeanDeadCoral,
                GBRCoralPointID
            });
        }
    }
}

