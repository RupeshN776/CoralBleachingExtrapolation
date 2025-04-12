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
/// 04-02-2025  2.0              Rupesh      Create Read Random polygon. Add button to home/read.cshtml. Write ReadRandomPolygon
/// 04-02-2025  2.0.1            Rupesh      Create Polygon - get and post according to the info and create cashtml
/// 04-04-2025  2.1              Ben         Create GetLatestPolygonJson
/// 04-11-2025   2.3             Ben         Add GoogleApiSettings 
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

    //--------------------------------The cover page----------------------------------//
    public IActionResult Index()
    {

        return View();
    }

    //--------------------------------The page that get random polygon ----------------------------------// PROMOTE: MISSING TRY CATACH WHY? //only used in assembly
    public IActionResult Read()
    {
        ViewBag.GoogleApiKey = _googleApiSettings.ApiKey;
        return View();//return
    }



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
    //--------------------------------Delete(The functionality is disable)----------------------------------//

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

    //--------------------------------New Page----------------------------------//
    public IActionResult New()
    {
        return View();
    }
    //--------------------------------Read Random Polygon----------------------------------// Rupesh

    [HttpGet]
    public IActionResult GetRandomPolygonJson()
    {
        var randomCoral = _db.tbl_GlobalCoralPolygon
                             .OrderBy(r => Guid.NewGuid())
                             .FirstOrDefault(); //get random polygon from the database. 
                                                //for some reason it doesnt work as well if i do it with the entity model for some reason

        if (randomCoral == null || randomCoral.Shape == null)
        {
            return Json(new { success = false, message = "No valid coral polygon found." }); //check if polygon is even there
        }

        string wkt = WorldCoral.GetWKTFromPolygon(randomCoral.Shape); //convert shape from db to wkt string because it that is easier since we use Polygon object in the World.cs


        string polygonJson = WorldCoral.ConvertWKTToLatLng(wkt); // since google polygon requires str we use the convert wkt to lat lng in World.cs to convert

        return Json(new
        {
            success = true,
            polygon = polygonJson, // 
            coralName = randomCoral.CoralName,
            originName = randomCoral.OriginName,
            family = randomCoral.Family,
            genus = randomCoral.Genus,
            species = randomCoral.Species,
            gisAreaKm2 = randomCoral.GISAREAKM2,
            regionId = randomCoral.RegionFK,
            id = randomCoral.GlobalCoralId
        }); // this will be shown in the infowindow 
    }


    //--------------------------------Read Polygon By ID----------------------------------//

    [HttpGet]
    public IActionResult GetLatestPolygonJson()
    {
        var coral = _db.tbl_GlobalCoralPolygon
                   .OrderByDescending(c => c.GlobalCoralId)
                   .FirstOrDefault();

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
    /// <summary>
    /// This entire process is based on the logic from Rupesh's lab 3 in last semester where i trace a polygon then get the distance and bearing and then apply that dist and bearing to a new coordiante
    /// </summary>
    /// <param name="globalCoralName"></param>
    /// <returns></returns>
    /// 
    //this is the get and connects to info.csthml, works when coral button is clicked 
    [HttpGet]
    public IActionResult Create(string globalCoralName)
    {
        if (globalCoralName == null)
        {
            return BadRequest("Invalid coral."); //check is broken
        }

        var coralEntity = _db.tbl_GlobalCoralPolygon
                             .FirstOrDefault(x => x.CoralName == globalCoralName); //so since the coral name is unqiue as well, we can just search by cornal Name because apprently, DML insertion is random so if we do it by ID, it messes it up

        if (coralEntity == null || coralEntity.Shape == null)
        {
            return NotFound("Base polygon not found for the selected coral."); //check if exists in database. 
        }

        //same as earlier, get polygon
        var basePolygonWKT = WorldCoral.GetWKTFromPolygon(coralEntity.Shape);


        //
        var model = new CoralCreationViewModel //we create a seperate model to store the create values. 
        {
            CoralType = coralEntity.CoralName,
            BasePolygonWKT = basePolygonWKT
        };
        ViewBag.GoogleApiKey = _googleApiSettings.ApiKey; //localized api key

        return View(model);
    }


    //--------------------------------Create Mirror Polygon----------------------------------//
    [HttpPost]
    public IActionResult Create(string CoralType, string CoralName, string BasePolygonWKT, double CenterLat, double CenterLng,
                         string OriginName, string Family, string Genus, string Species) //this takes in input from the create.cshtml
    {
        try
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var wktReader = new WKTReader(geometryFactory);

            if (string.IsNullOrWhiteSpace(BasePolygonWKT))
            {
                ModelState.AddModelError("", "Polygon data is missing."); //check missing data
                return View(new CoralCreationViewModel
                {
                    CoralType = CoralType,
                    BasePolygonWKT = BasePolygonWKT
                });
            }

            var originalPolygon = (Polygon)wktReader.Read(BasePolygonWKT); //get original polygon based on click from database (comes from info.cshtml)
            var center = new Coordinate(CenterLng, CenterLat);
            var shiftedPolygon = ShiftPolygonToCenter(originalPolygon, center); //move polygon to diff coords

            int regionFK = GetRegionFK(CenterLat, CenterLng); //get region int based on lat long

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
            }; //add to world coral model as we need to save to db

            _db.tbl_GlobalCoralPolygon.Add(obj);
            _db.SaveChanges();

            return RedirectToAction("New"); //if saves to db go back to home
        }
        catch (Exception ex) //if breaks due to 
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
        if (latitude > -30 && latitude < 30) //tbh this is not accurate at all, just a bit better then not being there
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

    private Polygon ShiftPolygonToCenter(Polygon original, Coordinate newCenter) //move polygon to center - inspired by clay 
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



    ///////////////Orignally i was going to create a individual controller for each polygon //////////////


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
