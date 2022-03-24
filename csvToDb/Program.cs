using System;
using System.Collections.Generic;
using LiteDB;
using Microsoft.VisualBasic.FileIO;

namespace csvToDb
{
    class Program
    {
        static void Main(string[] args)
        {
            // En lista av klassen Survey
            List<Survey> SurveyList = new List<Survey>();


            ////// CSV - TextFieldParser
            using (var reader = new TextFieldParser("SurveyForm.csv"))
            {
                reader.SetDelimiters(new string[] { "," });

                reader.ReadLine(); // Läs och glöm bort första raden

                while (!reader.EndOfData)
                {
                    try
                    {
                    string[] columns = reader.ReadFields();
                    SurveyList.Add(new Survey(int.Parse(columns[1]), columns[2], columns[3], int.Parse(columns[4])));

                    }catch (Exception){
                        Console.WriteLine("line x is invalid. skipping");
                    }

                }

            }

            ////// Exempel på forloop in i listan
            //foreach (Survey surveys in SurveyList)
            //{
            //    Console.WriteLine($"{surveys.MeatWeeks}, {surveys.KindOfFood}, {surveys.HomeOrRestaurant}, {surveys.LunchTime}");
            //}
            //    Console.WriteLine("Done");



            //// Läsa in objekten till databasen


            //// --------------
            //// NoSQL-databaser!

            //// Anslut till databasen (i filen MyData.db)
            using (var db = new LiteDatabase("MyData.db"))
            {
                // All data är organiserad i "collections"
                var surveyDB = db.GetCollection<Survey>("SurveyForm");


                // Spara undan min variabel ovanför
                surveyDB.DeleteAll(); // raderar allt Sen raden under lägger vi om allt på nytt så det inte blir dubletter, det kan finnas en bättre lösning
                surveyDB.Insert(SurveyList); // serialiserar objektet till JSON och sparar undan det 
                                             // som en del av MyData.db
                var result = surveyDB.Query().Where(x => x.LunchTime == 12).Select(x => new { x.HomeOrRestaurant, x.LunchTime }).ToList();// la till en test så vi ser att data finns i data basen och hämtar alla som har lunchTid vid 12
                Console.WriteLine(result); // vi ser här att svaret blir 9 av 13 som har lunch vid 12.
                Console.WriteLine("Klar");
            }

            /*// Del 2: Anslut till databasen igen och se vad som finns där
            using (var db = new LiteDatabase("MyData.db"))
            {
                var SurveyDB = db.GetCollection<Survey>("SurveyForm");

                var antal = SurveyDB.Count();
                var contents = SurveyDB.FindAll();

                Console.WriteLine("Det finns såhär många saker i min samling: " + antal);

            }
            */
        }
    }
}
