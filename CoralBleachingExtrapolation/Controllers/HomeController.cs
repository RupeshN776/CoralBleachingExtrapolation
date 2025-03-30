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
/// 24-3-2025   1.0              Ben         I go to school by bus
/// </summary>



public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    List<WorldCoral> TheModel;

    public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }

    //--------------------------------Read All----------------------------------//
    public IActionResult Index() // Read
    {
        //TheModel = _db.tbl_GlobalCoralPolygon.ToList();

        TheModel = _db.tbl_GlobalCoralPolygon
                           .OrderByDescending(x => x.GlobalCoralId) // Sort by ID in descending order
                           .Take(10)                     // Get the last 10 records
                           .ToList();

        return View(TheModel);
    }

    //--------------------------------Read One:TODO----------------------------------//
    public IActionResult Read(int id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        WorldCoral? Coral = _db.tbl_GlobalCoralPolygon.Find(id);

        if (Coral == null)
        {
            return NotFound();
        }
        return View(Coral);
    }


    //--------------------------------Create----------------------------------//
    public IActionResult Create() //Create
    {
        return View();  
    }

    [HttpPost]
    public IActionResult Create(WorldCoral obj) //Create
    {
            //test only
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
            return View();
    }


    //--------------------------------Update----------------------------------//
    [HttpGet]
    public IActionResult Update(int? id) // Update
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        WorldCoral? Coral = _db.tbl_GlobalCoralPolygon.Find(id);

        if (Coral == null)
        {
            return NotFound();
        }
        return View(Coral);
    }

    [HttpPost]
    public IActionResult Update(WorldCoral obj, string ShapeWkt)
    {
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
    //--------------------------------Delete:TODO----------------------------------//

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

    //--------------------------------TODO: CORAL POLYGON VIEW FOR MAP----------------------------------//

    //public IActionResult ViewCoral(int id)
    //{
    //    // Fetch coral from the database
    //    var coral = _db.tbl_GlobalCoralPolygon
    //        .Where(c => c.GlobalCoralId == id)
    //        .FirstOrDefault();

    //    // Check if coral is found
    //    if (coral == null)
    //    {
    //        return NotFound();  // Return 404 if no coral is found
    //    }

    //    // Assuming coral has a Shape (Polygon) with Coordinates
    //    var coordinates = new List<object>();

    //    if (coral.Shape != null)
    //    {
    //        var points = coral.Shape.STAsText().Value;

    //        // Parse WKT (Well-Known Text) into coordinates (for polygons)
    //        var matches = Regex.Matches(points, @"\(([^)]*)\)");  // Regular expression to extract coordinates
    //        foreach (var match in matches)
    //        {
    //            var coords = match.Groups[1].Value.Split(',');  // Coordinates are separated by a comma
    //            foreach (var coord in coords)
    //            {
    //                var latLon = coord.Split(' ');
    //                var latitude = Convert.ToDouble(latLon[0].Trim());
    //                var longitude = Convert.ToDouble(latLon[1].Trim());
    //                coordinates.Add(new { Latitude = latitude, Longitude = longitude });
    //            }
    //        }
    //    }

    //    // Pass coordinates to the view using ViewBag
    //    ViewBag.CoralCoordinates = coordinates;

    //    return View(coral);
    //}





}
