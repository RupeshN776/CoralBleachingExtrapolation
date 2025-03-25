using CoralBleachingExtrapolation.Data;
using CoralBleachingExtrapolation.Models;
using CoralBleachingExtrapolation.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using NetTopologySuite.Algorithm;
using System.Text.Json.Serialization;
using System.Text.Json;

/// <summary>
/// Date         Version     Name       Comment
/// 03-03-2025   1.0         Keelin     Create, Read, Update, Delete Intial
/// 03-15-2025   1.0.1       Keelin     bug fixes
/// 03-22-2025   1.0.2       Keelin     more bug fixes
/// 03-22-2025   PROMOTE     Rupesh     PROMOTE TESTING and COMMENTS
/// </summary>
/// 



namespace CoralBleachingExtrapolation.Controllers
{
    public class GBRController : Controller
    {
        private readonly ApplicationDbContextGBR _db;

        List<GBRCoralPoint> TheModel;

        public GBRController(ApplicationDbContextGBR db)
        {
            _db = db;
        }

        //--------------------------------Read All----------------------------------//
        public IActionResult Index() // Read
        {
            //TheModel = _db.tbl_GlobalCoralPolygon.ToList();

            TheModel = _db.tbl_GBRCoralPoint
                               .OrderByDescending(x => x.GBRCoralPointID) // Sort by ID in descending                   // Get the last 10 records
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

            GBRCoralPoint? Coral = _db.tbl_GBRCoralPoint.Find(id);

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

        public IActionResult Create(GBRCoralPoint obj)  //does not check report year input. //default lat lng are antartica
        {

            obj.Latitude = 1;
            obj.Longitude = 1;


            string wkt = "POINT (-75.0 40.0)"; // WKT for a single point //why in antartica

            try
            {
                var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
                var wktReader = new WKTReader(geometryFactory);
                Geometry geometry = wktReader.Read(wkt);

                if (geometry is Point point)
                {
                    obj.Point = point;
                    obj.Point.SRID = 4326; 
                    _db.tbl_GBRCoralPoint.Add(obj);
                    _db.SaveChanges();
                }
                else
                {
                    Console.WriteLine("The WKT string does not represent a valid Point.");
                    return View("Error");
                }

                return View();
            }
            catch (NetTopologySuite.IO.ParseException ex)
            {
                Console.WriteLine($"Error parsing WKT: {ex.Message}");
                return View("Error");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return View("Error");
            }
        }



        //--------------------------------Update----------------------------------//
        //GBR Update - does not test input year, WKT format does not reflect updated Lat Long.
        //GBR update - breaks upon invalid lat lng input


                [HttpGet]
        public IActionResult Update(int? id) // Update ??PROMTE NOTE: purpose for overload? name differently perhaps? 
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            GBRCoralPoint? Coral = _db.tbl_GBRCoralPoint.Find(id);

            if (Coral == null)
            {
                return NotFound();
            }
            return View(Coral);
        }

        [HttpPost]
        public IActionResult Update(GBRCoralPoint obj, string ShapeWkt) 
        {
            if (!string.IsNullOrEmpty(ShapeWkt))
            {
                try
                {
                    var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326); // SRID 4326 for geography
                    var reader = new WKTReader(geometryFactory);
                    obj.Point = (Point)reader.Read(ShapeWkt); // Convert WKT to Polygon
                    obj.Point.SRID = 4326;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Point", "Invalid point format.");
                }
            }

            if (ModelState.IsValid)
            {
                _db.tbl_GBRCoralPoint.Update(obj);
                _db.SaveChanges(); //error when lat or long input is invalid CHECK
                return RedirectToAction("Index");
            }

            return View(obj);
        }
        //--------------------------------Delete:TODO----------------------------------// 
        //DID NOT TEST - non-functional

        public IActionResult Delete(GBRCoralPoint obj) //Delete
        {
            _db.tbl_GBRCoralPoint.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
