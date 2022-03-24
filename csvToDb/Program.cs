using LiteDB;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;

namespace csvToDb
{
    internal class Program
    {
        private static void Main(string[] args)
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
                    }
                    catch (Exception)
                    {
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
                // serialiserar objektet till JSON och sparar undan det
                surveyDB.Insert(SurveyList); 
                // som en del av MyData.db
                // Fråga 1, hur många har svarat på formuläret
                var answer = surveyDB.Count();
                Console.WriteLine(answer);
                // Fråga 2, medelvärdet av kött konsumtionen

                // Fråga 3, Populäraste typen av mat

                // Fråga 4, hur många av alla som svarat på formuläret äter kött

                // Fråga 5, hur många som svarade på formuläret äter oftast hemma
                var result = surveyDB.Query().Where(x => x.LunchTime == 12).Count();
                // la till en test så vi ser att data finns i data basen och hämtar alla som har lunchTid vid 12
                Console.WriteLine($"Antalet personer som äter lunch vid 12 är {result} stycken"); // vi ser här att svaret blir 9 av 13 som har lunch vid 12.
                /* medelvärdet av kött konsumtion, hur många av alla som svarat på formuläret äter kött, populäraste typen av mat, hur många har svarat på formuläret,
               hur många som svarade på formuläret äter helre hemma */
            }
        }
    }
}