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
                        string[] columns = reader.ReadFields(); // Gör om raderna till arrayer.
                        SurveyList.Add(new Survey(int.Parse(columns[1]), columns[2], columns[3], int.Parse(columns[4]))); // Lägger varje index av kolumerna i listan
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
                var answer = surveyDB.Count(); 
                Console.WriteLine(answer);
                // Fråga 2, medelvärdet av kött konsumtionen

                // Fråga 3, Populäraste typen av mat

                // Fråga 4, hur många av alla som svarat på formuläret äter kött

                // Fråga 5, hur många som svarade på formuläret äter oftast hemma

                // Fråga 6, hur många som äter vid klockan 12.
                var result = surveyDB.Query().Where(x => x.LunchTime == 12).Count();
                
                Console.WriteLine($"Antalet personer som äter lunch vid 12 är {result} stycken");

            }
        }
    }
}