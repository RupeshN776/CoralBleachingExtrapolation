using Azure.Core.GeoJson;
using System.ComponentModel.DataAnnotations;
using NetTopologySuite.Geometries;
using NetTopologySuite.Algorithm;
using NetTopologySuite.Geometries;
using Microsoft.SqlServer.Types;


/// <summary>
/// 02-02-2025  1.0     Ben      Create WorldCoral Model
/// 03-24-2025  1.1     Ben      Json Serialize
/// </summary>
/// 

namespace CoralBleachingExtrapolation.Models
{
    public class WorldCoral
    {
        [Key]
        public int GlobalCoralId { get; set; }

        public string? CoralName { get; set; }

        public Polygon? Shape { get; set; }

        public string? OriginName { get; set; }
        public string? Family { get; set; }
        public string? Genus { get; set; }
        public string? Species { get; set; }
        public double? GISAREAKM2 { get; set; }
        public int? RegionFK { get; set; }

        //lamda if we find out we need it when we implementing the html stuff

        public static string GetWKTFromPolygon(Polygon polygon)
        {
            return polygon?.ToText(); // Converts the Polygon object to WKT
        }
        public static string ConvertWKTToLatLng(string wkt)
        {
            if (string.IsNullOrEmpty(wkt)) return "[]";

            try
            {
                string coordinatesPart = wkt.Replace("POLYGON ((", "").Replace("))", "");
                var coordinatePairs = coordinatesPart.Split(',');

                var latLngList = coordinatePairs.Select(pair =>
                {
                    var coords = pair.Trim().Split(' ');
                    double lng = double.Parse(coords[0]); // X
                    double lat = double.Parse(coords[1]); // Y
                    return new { lat, lng };
                }).ToList();

                return System.Text.Json.JsonSerializer.Serialize(latLngList);
            }
            catch
            {
                return "[]"; // Fallback in case of malformed WKT
            }
        }

    }

   
}
