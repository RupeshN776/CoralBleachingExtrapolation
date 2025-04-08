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

/// <summary>
/// Date        Version          Name        Comment
/// 25-2-2025   1.0              Ben         Create, Read, Update, Delete template
/// 03-22-2025  PROMOTE          Rupesh      PROMOTE TESTING and COMMENTS
/// 27-3-2025   1.1              Ben         Bug fixes based on promote comments
/// </summary>



public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    List<WorldCoral> TheModel;

    public HomeController(ApplicationDbContext db)
    {
        _db = db;
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
        return View();//return
    }


    //--------------------------------Create----------------------------------//
    public IActionResult Create() //Create
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(WorldCoral obj) //Create PROMOTE: TRY CATCH THIS BRO
    {
        //test only -promote: finalized this as we need to create standardized ones next sprint. 
        //finalized - standard polygons in progress. 
        //P: okay

        obj.RegionFK = 1;
        obj.GISAREAKM2 = 1;
        string wkt = "POLYGON((-75.0 40.0, -73.0 40.5, -74.0 41.0, -75.0 40.0))";
        var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        var wktReader = new WKTReader(geometryFactory);
        Polygon polygon = (Polygon)wktReader.Read(wkt);
        obj.Shape = polygon;
        obj.Shape.SRID = 4326;
        //test only
        _db.tbl_GlobalCoralPolygon.Add(obj);
        _db.SaveChanges();
        return RedirectToAction("Index");
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




    }
