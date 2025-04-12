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
using Microsoft.Extensions.Options;


/// <summary>
/// Date         Version     Name       Comment
/// 03-03-2025   1.0         Keelin     Create, Read, Update, Delete Intial
/// 03-15-2025   1.0.1       Keelin     bug fixes
/// 03-22-2025   1.0.2       Keelin     more bug fixes
/// 03-22-2025   PROMOTE     Rupesh     PROMOTE TESTING and COMMENTS
/// 03-24-2025   1.0.3       Keelin     bug fixes based on promote code  
/// 04-02-2025   2.1         Ben        Revamp Index and Create to fix the current frontend design
/// 04-04-2025   2.2         Ben        Fix update Bug
/// 04-11-2025   2.3         Ben        Add GoogleApiSettings 
/// </summary>
/// 



namespace CoralBleachingExtrapolation.Controllers
{
    public class GBRController : Controller
    {
        private readonly ApplicationDbContextGBR _db;
        private readonly GoogleApiSettings _googleApiSettings;

        List<GBRCoralPoint> TheModel;

        public GBRController(ApplicationDbContextGBR db, IOptions<GoogleApiSettings> googleApiOptions)
        {
            _db = db;
            _googleApiSettings = googleApiOptions.Value;
        }

        //--------------------------------Read All----------------------------------//
        public IActionResult Index() // Read
        {
            TheModel = _db.tbl_GBRCoralPoint.ToList();
            ViewBag.GoogleApiKey = _googleApiSettings.ApiKey;

            return View(TheModel);
        }



        //--------------------------------Create----------------------------------//
        public IActionResult Create() //Create
        {
            ViewBag.GoogleApiKey = _googleApiSettings.ApiKey;

            return View();
        }


        [HttpPost]
        public IActionResult Create(GBRCoralPoint obj)
        {
            // Validate ReportYear
            if (obj.ReportYear < 1980 || obj.ReportYear > 2025)
            {
                ModelState.AddModelError("ReportYear", "Report Year must be between 1980 and 2025.");
            }

            // Validate Latitude and Longitude
            if (obj.Latitude < -90 || obj.Latitude > 90)
            {
                ModelState.AddModelError("Latitude", "Latitude must be between -90 and 90.");
            }

            if (obj.Longitude < -180 || obj.Longitude > 180)
            {
                ModelState.AddModelError("Longitude", "Longitude must be between -180 and 180.");
            }

            // Return form with errors if validation fails
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            try
            {
                var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
                string wkt = $"POINT ({obj.Longitude} {obj.Latitude})";
                var wktReader = new WKTReader(geometryFactory);
                Geometry geometry = wktReader.Read(wkt);

                if (geometry is Point point)
                {
                    obj.Point = point;
                    obj.Point.SRID = 4326;

                    _db.tbl_GBRCoralPoint.Add(obj);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("Point", "The provided coordinates do not form a valid point.");
                    return View(obj);
                }
            }
            catch (NetTopologySuite.IO.ParseException ex)
            {
                ModelState.AddModelError("Point", $"Error parsing coordinates: {ex.Message}");
                return View(obj);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Unexpected error: {ex.Message}");
                return View(obj);
            }
        }




        //--------------------------------Update----------------------------------//
        //GBR Update - does not test input year, WKT format does not reflect updated Lat Long. //sprint 5 assigned. 
        //GBR update - breaks upon invalid lat lng input


        [HttpGet]
        public IActionResult Update(int? id) // Update ??PROMTE NOTE: purpose for overload? name differently perhaps? //only used for button interaction on client side. //okay
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
        public IActionResult Update(GBRCoralPoint obj, string ShapeWkt)  //PROMOTE: CHECK LAT LONG INPUT AND REPORT YEAR. //fixed //P: WKT DOES NOT REFLECT LAT LNG CHANGES. //agreed to do next sprint if not possible now. 
        {
            // check lat
            if (obj.Latitude < -90 || obj.Latitude > 90)
            {
                ModelState.AddModelError("Latitude", "Invalid Input. Latitude must be between -90 and 90."); //check lat
            }
            //check long
            if (obj.Longitude < -180 || obj.Longitude > 180)
            {
                ModelState.AddModelError("Longitude", "Invalid Input. Longitude must be between -180 and 180."); //check long
            }

            if (obj.ReportYear < 1980 || obj.ReportYear > 2025)
            {
                ModelState.AddModelError("ReportYear", "Report Year must be between 1980 and 2025."); //check report year
            }

            if (!string.IsNullOrEmpty(ShapeWkt))
            {
                try
                {
                    var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326); // SRID 4326 for geography
                    var reader = new WKTReader(geometryFactory);

                    obj.Point = (Point)reader.Read(ShapeWkt); // Convert WKT to Polygon

                    string newWkt = $"POINT ({obj.Longitude} {obj.Latitude})"; //refelct the updated lat long. 

                    obj.Point = (Point)reader.Read(newWkt);
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
                _db.SaveChanges(); //P: error when lat or long input is invalid CHECK
                return RedirectToAction("Index");
            }

            return View(obj); //keeps the loop if they input incorrect values. 
        }
        //--------------------------------Delete:TODO----------------------------------// 
        //PROMOTE: DID NOT TEST - non-functional

        public IActionResult Delete(GBRCoralPoint obj) //Delete
        {
            _db.tbl_GBRCoralPoint.Remove(obj);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
