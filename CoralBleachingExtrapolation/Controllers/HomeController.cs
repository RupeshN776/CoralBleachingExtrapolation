using CoralBleachingExtrapolation.Data;
using CoralBleachingExtrapolation.Models;
using CoralBleachingExtrapolation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.Algorithm;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using NetTopologySuite;
using Microsoft.Extensions.Options;

/// <summary>
/// Date        Version          Name        Comment
/// 25-2-2025   1.0              Ben         Create, Read, Update, Delete template
/// 03-22-2025  PROMOTE          Rupesh      PROMOTE TESTING and COMMENTS
/// 27-3-2025   1.1              Ben         Bug fixes based on promote comments
/// </summary>



public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly GoogleApiSettings _googleApiSettings;
    List<WorldCoral> TheModel;

    public HomeController(ApplicationDbContext db, IOptions<GoogleApiSettings> googleApiOptions)
    {
        _db = db;
        _googleApiSettings = googleApiOptions.Value;
    }

    //PROMOTE: COMMENT ALL

    //--------------------------------Read All----------------------------------//
    public IActionResult Index()
    {
        //TheModel = _db.tbl_GlobalCoralPolygon.ToList();

        //TheModel = _db.tbl_GlobalCoralPolygon
        //                   .OrderByDescending(x => x.GlobalCoralId) // Sort by ID in descending order
        //                   .Take(10)                     // Get the last 10 records
        //                   .ToList();

        return View();
    }

    //--------------------------------Read One:TODO----------------------------------// PROMOTE: MISSING TRY CATACH WHY? //only used in assembly
    public IActionResult Read()
    {
        ViewBag.GoogleApiKey = _googleApiSettings.ApiKey;
        return View();//return
    }


    //--------------------------------Create----------------------------------//
    //public IActionResult Create() //Create
    //{
    //    return View();
    //}

    //[HttpPost]
    //public IActionResult Create(WorldCoral obj) //Create PROMOTE: TRY CATCH THIS BRO
    //{
    //    //test only -promote: finalized this as we need to create standardized ones next sprint. 
    //    //finalized - standard polygons in progress. 
    //    //P: okay

    //    obj.RegionFK = 1;
    //    obj.GISAREAKM2 = 1;
    //    string wkt = "POLYGON((-75.0 40.0, -73.0 40.5, -74.0 41.0, -75.0 40.0))";
    //    var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
    //    var wktReader = new WKTReader(geometryFactory);
    //    Polygon polygon = (Polygon)wktReader.Read(wkt);
    //    obj.Shape = polygon;
    //    obj.Shape.SRID = 4326;
    //    //test only
    //    _db.tbl_GlobalCoralPolygon.Add(obj);
    //    _db.SaveChanges();
    //    return RedirectToAction("Index");
    //}


    //--------------------------------Update----------------------------------//
    [HttpGet]
    public IActionResult Update(int? id) // Update
    {
        if (id == null || id == 0)
        {
            return NotFound(); //check valid id
        }

        WorldCoral? Coral = _db.tbl_GlobalCoralPolygon.Find(id);

        if (Coral == null)
        {
            return NotFound(); //check coral object
        }
        return View(Coral);
    }

    [HttpPost]
    public IActionResult Update(WorldCoral obj, string ShapeWkt) ///PROMOTE: DOES NOT CHECK AREA INPUT //fixed
    {
        if (obj.GISAREAKM2 < 0 || obj.GISAREAKM2 > 9858.92)
        {
            ModelState.AddModelError("GISAREAKM2", "Invalid Input."); //control the area input for a polygon

        }


        if (!string.IsNullOrEmpty(ShapeWkt))
        {
            try
            {
                var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326); // SRID 4326 for geography
                var reader = new WKTReader(geometryFactory);
                obj.Shape = (Polygon)reader.Read(ShapeWkt); // Convert WKT to Polygon
                obj.Shape.SRID = 4326;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Shape", "Invalid shape format.");
            }
        }

        if (ModelState.IsValid)
        {
            _db.tbl_GlobalCoralPolygon.Update(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        return View(obj);
    }
    //--------------------------------Delete:TODO----------------------------------// PROMOTE: THROWS ERROR, REMOVE FROM CLIENT SIDE || FIX

    public IActionResult Delete(WorldCoral obj) //Delete
    {
        _db.tbl_GlobalCoralPolygon.Remove(obj);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }


    //--------------------------------Info Page----------------------------------//
    public IActionResult Info()
    {
        return View();
    }

    //--------------------------------Create Page----------------------------------//
    public IActionResult Create()
    {
        ViewBag.GoogleApiKey = _googleApiSettings.ApiKey;
        return View();
    }

    //--------------------------------Read Random Polygon----------------------------------//

    [HttpGet]
    public IActionResult GetRandomPolygonJson()
    {
        var randomCoral = _db.tbl_GlobalCoralPolygon
                             .OrderBy(r => Guid.NewGuid())
                             .FirstOrDefault();

        if (randomCoral == null || randomCoral.Shape == null)
        {
            return Json(new { success = false, message = "No valid coral polygon found." });
        }

        string wkt = WorldCoral.GetWKTFromPolygon(randomCoral.Shape);
        string polygonJson = WorldCoral.ConvertWKTToLatLng(wkt);

        return Json(new
        {
            success = true,
            polygon = polygonJson,
            coralName = randomCoral.CoralName,
            originName = randomCoral.OriginName,
            family = randomCoral.Family,
            genus = randomCoral.Genus,
            species = randomCoral.Species,
            gisAreaKm2 = randomCoral.GISAREAKM2,
            regionId = randomCoral.RegionFK,
            id = randomCoral.GlobalCoralId
        });
    }


    //--------------------------------Read Polygon By ID----------------------------------//

    [HttpGet]
    public IActionResult GetPolygonById(int id)
    {
        var coral = _db.tbl_GlobalCoralPolygon.FirstOrDefault(c => c.GlobalCoralId == id);

        if (coral == null || coral.Shape == null)
        {
            return Json(new { success = false, message = "Coral polygon not found." });
        }

        string wkt = WorldCoral.GetWKTFromPolygon(coral.Shape);
        string polygonJson = WorldCoral.ConvertWKTToLatLng(wkt);

        string regionName = coral.RegionFK switch
        {
            1 => "Atlantic Ocean",
            2 => "Indian Ocean",
            3 => "Pacific Ocean",
            _ => "Unknown Region"
        };

        return Json(new
        {
            success = true,
            polygon = polygonJson,
            coralName = coral.CoralName,
            originName = coral.OriginName,
            family = coral.Family,
            genus = coral.Genus,
            species = coral.Species,
            gisAreaKm2 = coral.GISAREAKM2,
            regionId = coral.RegionFK,
            regionName = regionName,
            id = coral.GlobalCoralId
        });
    }


    //--------------------------------Create Mirror Polygon By ID----------------------------------//
    [HttpGet]
    public IActionResult Create(int globalCoralId)
    {
        if (globalCoralId <= 0)
        {
            return BadRequest("Invalid coral ID.");
        }

        var coralEntity = _db.tbl_GlobalCoralPolygon
                             .FirstOrDefault(x => x.GlobalCoralId == globalCoralId);

        if (coralEntity == null || coralEntity.Shape == null)
        {
            return NotFound("Base polygon not found for the selected coral.");
        }

        var basePolygonWKT = WorldCoral.GetWKTFromPolygon(coralEntity.Shape);

        var model = new CoralCreationViewModel
        {
            CoralType = coralEntity.CoralName,
            BasePolygonWKT = basePolygonWKT
        };
        ViewBag.GoogleApiKey = _googleApiSettings.ApiKey;

        return View(model);
    }


    //--------------------------------Create Mirror Polygon----------------------------------//
    [HttpPost]
    public IActionResult Create(string CoralType, string CoralName, string BasePolygonWKT, double CenterLat, double CenterLng,
                         string OriginName, string Family, string Genus, string Species)
    {
        try
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var wktReader = new WKTReader(geometryFactory);

            if (string.IsNullOrWhiteSpace(BasePolygonWKT))
            {
                ModelState.AddModelError("", "Polygon data is missing.");
                return View(new CoralCreationViewModel
                {
                    CoralType = CoralType,
                    BasePolygonWKT = BasePolygonWKT
                });
            }

            var originalPolygon = (Polygon)wktReader.Read(BasePolygonWKT);
            var center = new Coordinate(CenterLng, CenterLat);
            var shiftedPolygon = ShiftPolygonToCenter(originalPolygon, center);

            int regionFK = GetRegionFK(CenterLat, CenterLng);

            var obj = new WorldCoral
            {
                CoralName = CoralName,
                OriginName = OriginName,
                Family = Family,
                Genus = Genus,
                Species = Species,
                GISAREAKM2 = null,
                RegionFK = regionFK,
                Shape = shiftedPolygon
            };

            _db.tbl_GlobalCoralPolygon.Add(obj);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "Error: " + ex.Message);
            return View(new CoralCreationViewModel
            {
                CoralType = CoralType,
                BasePolygonWKT = BasePolygonWKT
            });
        }
    }

    //--------------------------------Region check----------------------------------//
    private int GetRegionFK(double latitude, double longitude)
    {
        if (latitude > -30 && latitude < 30)
        {
            if (longitude < -50)
                return 1; // Atlantic Ocean
            if (longitude > 50)
                return 3; // Pacific Ocean
            return 2; // Indian Ocean
        }
        return 0; // Default or error region
    }

    //--------------------------------Change Coordinate For Polygon----------------------------------//

    private Polygon ShiftPolygonToCenter(Polygon original, Coordinate newCenter)
    {
        var originalCoords = original.Coordinates;
        var originalCentroid = original.Centroid.Coordinate;

        double deltaX = newCenter.X - originalCentroid.X;
        double deltaY = newCenter.Y - originalCentroid.Y;

        var shiftedCoords = originalCoords
            .Select(c => new Coordinate(c.X + deltaX, c.Y + deltaY))
            .ToArray();

        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        return geometryFactory.CreatePolygon(shiftedCoords);
    }


    ////--------------------------------Coraltype and correspoing arean----------------------------------//
    //private double? GetGISAreaByCoralType(string coralType)
    //{
    //    switch (coralType)
    //    {
    //        case "Staghorn Coral":
    //            return 50.0; // Example GIS Area for Staghorn Coral
    //        case "Elkhorn Coral":
    //            return 45.0; // Example GIS Area for Elkhorn Coral
    //                         // Add more cases for other coral types
    //        default:
    //            return 1.0; // Default GIS Area
    //    }
    //}





    //private Polygon GenerateCircularPolygon(Coordinate center, double radiusMeters, int numPoints = 36) //original based on RUpesh Lab 3 logic from Basic Programming
    //{
    //    var coords = new List<Coordinate>();
    //    var earthRadius = 6371000.0; // meters
    //    var lat = ToRadians(center.Y);
    //    var lng = ToRadians(center.X);

    //    for (int i = 0; i <= numPoints; i++)
    //    {
    //        var bearing = 2 * Math.PI * i / numPoints;

    //        var lat2 = Math.Asin(Math.Sin(lat) * Math.Cos(radiusMeters / earthRadius) +
    //                             Math.Cos(lat) * Math.Sin(radiusMeters / earthRadius) * Math.Cos(bearing));
    //        var lng2 = lng + Math.Atan2(Math.Sin(bearing) * Math.Sin(radiusMeters / earthRadius) * Math.Cos(lat),
    //                                    Math.Cos(radiusMeters / earthRadius) - Math.Sin(lat) * Math.Sin(lat2));

    //        coords.Add(new Coordinate(ToDegrees(lng2), ToDegrees(lat2)));
    //    }

    //    coords.Add(coords[0]); // Close polygon

    //    // Reverse coordinates to ensure counter-clockwise order
    //    coords.Reverse();

    //    var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
    //    return geometryFactory.CreatePolygon(coords.ToArray());
    //}

    //private double ToRadians(double degrees) => degrees * Math.PI / 180.0;
    //private double ToDegrees(double radians) => radians * 180.0 / Math.PI;






    //public IActionResult CreateFromStaghorn()
    //{
    //    int globalCoralId = 17166; // Replace with actual ID for Staghorn
    //    var basePolygon = _db.tbl_GlobalCoralPolygon.FirstOrDefault(x => x.GlobalCoralId == globalCoralId)?.Shape;

    //    if (basePolygon == null)
    //        return NotFound("Base polygon not found");

    //    var model = new CoralCreationViewModel
    //    {
    //        CoralType = "Staghorn Coral",
    //        BasePolygonWKT = WorldCoral.GetWKTFromPolygon(basePolygon)
    //    };

    //    return View("~/Views/WorldCoral/Create.cshtml", model);
    //}

}
