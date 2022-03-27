// Uppgiften är gjord av: Peyman Eliassi, Simon Hultqvist, Aymen Derraji

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

            //// Skapa / Anslut till databasen (MyData.db)
            using (var db = new LiteDatabase("MyData.db"))
            {
                // All data är organiserad i "collections"
                var surveyDB = db.GetCollection<Survey>("SurveyForm");

                // Spara undan min variabel ovanför
                surveyDB.DeleteAll(); // Vi rensar "SurveyForm" Collection ifall det finns redan information där så vi kan börja på nytt.
                surveyDB.Insert(SurveyList); // Vi lägger listan in i databasen.

                // Fråga 1, hur många har svarat på formuläret
                var answer = surveyDB.Count(); // Räknar de rader som finns i surveyDB
                Console.WriteLine($"Antal svar på formuläret: {answer}");

                // Fråga 2, genomsnitt hur många gånger kött äts per vecka
                double sum = 0;
                foreach (var meat in SurveyList) // går igeom survetList och adderar värdet MeatWeeks har
                {
                    sum += meat.MeatWeeks;
                }
                // Dividerar summan med answer, har redan räknat antal rader som finns mha variablen answer
                Console.WriteLine($"Genomsnitt kött konsumptionen per vecka: {sum / answer}");

                // Fråga 3, Populäraste typen av mat
                var popularFood = SurveyList.GroupBy(x => x.KindOfFood); // variabel med populär mat, grupperar element KindOfFood från listan surveyList
                var maxCount = popularFood.Max(g => g.Count());// Vi tar fram det maxvärdet.
                var mostPopular = popularFood.Where(x => x.Count() == maxCount).Select(x => x.Key).ToArray(); // Sedan kollar vi vilka eller vilken har max värdet och hämtar namnet med key.
                Console.WriteLine($"De mest populära typerna av mat är {mostPopular[0]} och {mostPopular[1]}");

                // Fråga 4, hur många av alla som svarat på formuläret äter kött
                var meatEating = surveyDB.Count(x => x.MeatWeeks > 0); // Räknar endast de som har svarat att de åtminstone äter kött en gång per vecka
                Console.WriteLine($"Antal individer som äter kött per vecka: {meatEating}");

                // Fråga 5, hur många som svarade på formuläret äter oftare ute
                var homeOrRestaurant = surveyDB.Query().Where(x => x.HomeOrRestaurant == "Äta ute!").Count(); // Hämtar "Äta ute!" från databasen och kör sedan en count.
                Console.WriteLine($"Antal individer som äter ute är: {homeOrRestaurant}");

                // Fråga 6, hur många som äter vid klockan 12.
                var result = surveyDB.Query().Where(x => x.LunchTime == 12).Count(); // hämtar hur många som äter vid klockan 12.
                Console.WriteLine($"Antalet personer som äter lunch vid 12 är {result} stycken");
            }
        }
    }
}