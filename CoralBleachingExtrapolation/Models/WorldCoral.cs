using Azure.Core.GeoJson;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;
using NetTopologySuite.Algorithm;

namespace CoralBleachingExtrapolation.Models
{
    public class WorldCoral
    {
        [Key]
        public int GlobalCoralId { get; set; }
        
        public string? CoralName { get; set; }

        public Polygon? Shape { get; set; }

        public string? OriginName {  get; set; }
        public string? Family { get; set; }
        public string? Genus { get; set; }
        public string? Species { get; set; }
        public double? GISAREAKM2 { get;set; }
        public int? RegionFK { get;set; }

        //lamda if we find out we need it when we implementing the html stuff
       


    }
}
