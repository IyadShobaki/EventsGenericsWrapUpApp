using System;
using System.Collections.Generic;
using System.IO;

namespace EventsGenericsWrapUp
{
   internal class Program
   {
      static void Main(string[] args)
      {
         List<PersonModel> people = new List<PersonModel>
         {
            new PersonModel {FirstName = "Iyad", LastName = "Shobaki", Email = "iyad@shobaki.com"},
            new PersonModel { FirstName = "Tim", LastName = "Corey", Email = "tim@iamtimcorey.com" },
            new PersonModel { FirstName = "Sue", LastName = "StormDarnit", Email = "sue@storm.com" }
         };

         List<CarModel> cars = new List<CarModel>
         {
            new CarModel {Manufacturer = "Toyota", Model = "Corolla"},
            new CarModel {Manufacturer = "Toyota", Model = "Highlander"},
            new CarModel {Manufacturer = "Ford", Model = "heckMustang"}

         };

         DataAcess<PersonModel> peopleData = new DataAcess<PersonModel>();
         peopleData.BadEntryFound += PeopleData_BadEntryFound;
         peopleData.SaveToCSV(people, @"C:\Temp\SavedFiles\people.csv");

         DataAcess<CarModel> carsData = new DataAcess<CarModel>();
         carsData.BadEntryFound += CarsData_BadEntryFound;
         carsData.SaveToCSV(cars, @"C:\Temp\SavedFiles\cars.csv");


         Console.ReadLine();
      }

      private static void CarsData_BadEntryFound(object sender, CarModel e)
      {
         Console.WriteLine($"Bad Entry found for {e.Manufacturer} {e.Model}");
      }

      private static void PeopleData_BadEntryFound(object sender, PersonModel e)
      {
         Console.WriteLine($"Bad Entry found for {e.FirstName} {e.LastName}");
      }
   }

   public class DataAcess<T> where T : new()
   {
      public event EventHandler<T> BadEntryFound;

      public void SaveToCSV(List<T> items, string filePath)
      {
         List<string> rows = new List<string>();

         T entry = new T();
         // Using reflection to know the type and the properties of the object
         var cols = entry.GetType().GetProperties();

         string row = "";
         foreach (var col in cols)
         {
            row += $",{col.Name}";
         }
         row = row.Substring(1);  //"FirstName,LastName,Email"
         rows.Add(row);

         foreach (var item in items)
         {
            row = "";
            bool badWordDetector = false;

            foreach (var col in cols)
            {
               string val = col.GetValue(item, null).ToString();

               badWordDetector = BadWordDetector(val);
               if (badWordDetector == true)
               {
                  BadEntryFound?.Invoke(this, item);
                  break;
               }
               row += $",{val}";
            }

            if (badWordDetector == false)
            {
               row = row.Substring(1);
               rows.Add(row);

            }
         }

         File.WriteAllLines(filePath, rows);
      }

      private bool BadWordDetector(string stringToTest)
      {
         bool output = false;

         string lowerCaseTest = stringToTest.ToLower();

         if (lowerCaseTest.Contains("darn") || lowerCaseTest.Contains("heck"))
         {
            output = true;
         }

         return output;
      }
   }
}
