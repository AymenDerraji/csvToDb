using LiteDB;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace csvToDb
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            // En lista av klassen Survey
            List<Survey> SurveyList = new List<Survey>();
            List<string> s = new List<string>();
            // CSV - TextFieldParser
            using (var reader = new TextFieldParser("SurveyForm.csv"))
            {
                reader.SetDelimiters(new string[] { "," }); // vi gör avgränsningar mellan varje komma tecken(obs kommatecken i " , " ignoreras.

                reader.ReadLine(); // Läs och glöm bort första raden

                // while loop så länge den inte har läst igenom all data text i csv filen.
                while (!reader.EndOfData)
                {
                    try // En try catch för att hantera felsökning lättare ifall det finns någon data text som inte går omvandla till rätt objekt av klassen.
                    {
                        // Gör om raderna till arrayer.
                        string[] columns = reader.ReadFields();
                        // Lägger varje index av kolumerna i listan
                        SurveyList.Add(new Survey(int.Parse(columns[1]), columns[2], columns[3], int.Parse(columns[4])));
                    }
                    catch (Exception error) // Kommer hit ifall den hittar ett fel!
                    {
                        Console.WriteLine($"Hoppsan! {error} hände när den försökte läsa en rad i filen."); // felmedelandet
                    }
                }
            }

            //// --------------
            //// NoSQL-databaser!

            //// Skapa / Anslut till databasen (MyData.db)
            using (var db = new LiteDatabase("MyData.db"))
            {
                // All data är organiserad i "collections"
                var surveyDB = db.GetCollection<Survey>("SurveyForm");

                // Spara undan min variabel ovanför
                surveyDB.DeleteAll(); // raderar allt Sen raden under lägger vi om allt på nytt så det inte blir dubletter, det kan finnas en bättre lösning
                surveyDB.Insert(SurveyList); // Vi lägger listan in i databasen.

                // Fråga 1, hur många har svarat på formuläret
                // Räknar de rader som finns i surveyDB
                var answer = surveyDB.Count();
                Console.WriteLine("Antal svar på formuläret: " + answer);
                // Fråga 2, genomsnitt hur många gånger kött äts per vecka
                double sum = 0;
                foreach (var meat in SurveyList) // går igeom survetList och adderar värdet MeatWeeks har
                {
                    sum += meat.MeatWeeks;
                }
                // Dividerar summan med answer, har redan räknat antal rader som finns mha variablen answer
                Console.WriteLine("Genomsnitt kött konsumptionen per vecka: " + sum / answer);
                // Fråga 3, Populäraste typen av mat
                // variabel med populär mat, grupperar element KindOfFood från listan surveyList
                var popularFood = SurveyList.GroupBy(x => x.KindOfFood);
                // sätter maxCount tar fram de typer av mat som har det högsta värdet, dvs det som repeterats flest gånger och räknar dem
                var maxCount = popularFood.Max(g => g.Count());
                // Sätter mostPopular till popularFood och räknar de gånger som popular food repeterats lika många gånger som maxCount. Vår ut värdet från popularFood genom
                // key och gör till array. SKulle man sätta mostPopular[2] får man error eftersom arrayen innehåller bara det mest populäraste mat typen vilket råkar vara 2 st,
                // dvs 2st mat typer har lika många "röster"
                var mostPopular = popularFood.Where(x => x.Count() == maxCount).Select(x => x.Key).ToArray();
                string sumOfMostPopular = "";
                for (int i = 0; i < mostPopular.Length; i++)
                {
                    if (i == 0)
                        sumOfMostPopular = mostPopular[i];
                    else
                        sumOfMostPopular += " och " + mostPopular[i];

                }
                Console.WriteLine("De mest populära typerna av mat är " + sumOfMostPopular);

                // Fråga 4, hur många av alla som svarat på formuläret äter kött
                // Räknar endast de som har svarat att de åtminstone äter kött en gång per vecka
                var meatEating = surveyDB.Count(x => x.MeatWeeks > 0);
                Console.WriteLine("Antal individer som äter kött per vecka: " + meatEating);

                // Fråga 5, hur många som svarade på formuläret äter oftare ute
                // Hämtar data från given data, specifierar vilket element från datan som ska returnera något
                var homeOrRestaurant = surveyDB.Query().Where(x => x.HomeOrRestaurant == "Äta ute!").Count();
                Console.WriteLine("Antal individer som äter ute är: " + homeOrRestaurant);
                // Fråga 6, hur många som äter vid klockan 12.
                var result = surveyDB.Query().Where(x => x.LunchTime == 12).Count();

                Console.WriteLine($"Antalet personer som äter lunch vid 12 är {result} stycken");
            }
        }
    }
}