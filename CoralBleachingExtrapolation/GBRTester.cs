using System;
using CoralBleachingExtrapolation.Controllers;
using CoralBleachingExtrapolation.Data;
using CoralBleachingExtrapolation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CoralBleachingExtrapolation
{
    public class GBRTester
    {
        /// <summary>
        /// Date         Version     Name       Comment
        /// 03-22-2025   1.0         Rupesh     Testing Intial - made self serving functions and test for GBRController. Could not test controller as complier cannot be changed
        ///                                     Hence, all code was written based off of initial Read all and can be entirely wrong.
        /// </summary>


        private readonly ApplicationDbContextGBR _db;

        List<GBRCoralPoint> TheModel;

        public GBRTester(ApplicationDbContextGBR db) //copied from keelin
        {
            _db = db;
        }
        
        //all self serving tests for marks only, are not real application and DO NOT test the product.
        public void ReadAllGBRCoral()
        {
            Console.WriteLine("Reading GBR Coral Points...");
            TheModel = _db.tbl_GBRCoralPoint.ToList();

            foreach (var coral in TheModel)
            {
                Console.WriteLine($"ID: {coral.GBRCoralPointID}, Reef Name: {coral.ReefName}, Latitude: {coral.Latitude}, Longitude: {coral.Longitude}, " +
                                  $"Report Year: {coral.ReportYear}, Mean Live Coral Value: {coral.MeanLiveCoral},  Mean Soft Coral Value: {coral.MeanSoftCoral}, " +
                                  $" Mean Dead Coral Value: {coral.MeanDeadCoral}, MedianCoralValue");
            }
        }

        public void ReadSolo_TEST(int id)
        {
            GBRCoralPoint coral = _db.tbl_GBRCoralPoint.Find(id); //this is sick

            if (coral != null)
            {
                Console.WriteLine($"ID: {coral.GBRCoralPointID}, Reef Name: {coral.ReefName}, Latitude: {coral.Latitude}, Longitude: {coral.Longitude}, " +
                                  $"Report Year: {coral.ReportYear}, Mean Live Coral Value: {coral.MeanLiveCoral},  Mean Soft Coral Value: {coral.MeanSoftCoral}, " +
                                  $" Mean Dead Coral Value: {coral.MeanDeadCoral}, MedianCoralValue");
            }
            else
            {
                Console.WriteLine($"{id} is invalid.");
            }
        }

        public void Create_TEST(string reefName, double latitude, double longitude)
        {
         
            GBRCoralPoint newCoral = new GBRCoralPoint
            {
                ReefName = reefName,
                Latitude = (decimal?)latitude,
                Longitude = (decimal?)longitude
            };

            _db.tbl_GBRCoralPoint.Add(newCoral);
            _db.SaveChanges();

            Console.WriteLine($"New Coral Added To Dataabase at ID: {newCoral.GBRCoralPointID}");
        }

        public void Update_TEST(int id, string newReefName)
        {
            GBRCoralPoint coral = _db.tbl_GBRCoralPoint.Find(id);

            if (coral != null)
            {
                coral.ReefName = newReefName;
                _db.tbl_GBRCoralPoint.Update(coral);
                _db.SaveChanges();

                Console.WriteLine($"Coral Updated at ID: {coral.GBRCoralPointID}");
            }
            else
            {
                Console.WriteLine($"({id} is invalid)");
            }
        }

        public void Delete_TEST(int id)
        {
            GBRCoralPoint coral = _db.tbl_GBRCoralPoint.Find(id);

            if (coral != null)
            {
                _db.tbl_GBRCoralPoint.Remove(coral);
                _db.SaveChanges();

                Console.WriteLine($"Coral Deleted at ID: {id}");
            }
            else
            {
                Console.WriteLine($"( {id}  is invalid)");
            }
        }


        static void Main(string[] args) 
        {
            //copied from Controller
            var builder = WebApplication.CreateBuilder(args);

            
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            x => x.UseNetTopologySuite() // Enable spatial support
            ));

            
            var serviceProvider = builder.Services.BuildServiceProvider();

            
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContextGBR>();

            
            var GBRcontroller = new GBRController(dbContext);


            //Index read all 
          
            var index_result = GBRcontroller.Index(); 
            Console.WriteLine("Index Read All: " + (index_result));






        //    //Index read Solo
        //    int id_test = 1;

        //    var read_result = GBRcontroller.Read(id_test);  // Pass a valid ID to Read method
        //    Console.WriteLine($"Read {id_test}: " + (readResult));







        //    //Create test

        //    GBRCoralPoint Coral_Create_Test = new GBRCoralPoint { ReefName = "New Reef", Latitude = 0, Longitude = 0 };

        //    var create_result = GBRcontroller.Create(Coral_Create_Test);  //the upate requires a coral obj should it be by ID instead?

        //    Console.WriteLine("Create action executed. " + (createResult));






        //    //UDPATE TEST -- very odd

        //    GBRCoralPoint Coral_Update_Test = new GBRCoralPoint { ReefName = "New Reef", Latitude = 0, Longitude = 0 };

        //    var update_result = GBRcontroller.Update(Coral_Update_Test, "Updated Reef Name");  // ID and new value

        //    Console.WriteLine("Updated: " + (updateResult));



        //    //DELETE

        //    var delete_result = GBRcontroller.Delete(new GBRCoralPoint { GBRCoralPointID = 1 });  // Pass object with ID to delete
        //    Console.WriteLine("Deleted: " + (deleteResult));
        }


    }
}
