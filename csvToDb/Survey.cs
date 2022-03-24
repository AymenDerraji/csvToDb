using System;
namespace csvToDb
{
        internal class Survey
        {
            // Skapar fields så vi kan senare använda i konstruktorn för att skapa objekt för datan i csv filen.
            public int MeatWeeks { get; set; }
            public string KindOfFood { get; set; }
            public string HomeOrRestaurant { get; set; }
            public int LunchTime { get; set; }

            // En konstruktor för klassen.
            public Survey(int weeks, string foodkind, string homerestaurant, int lunchtime)
            {
                MeatWeeks = weeks;
                KindOfFood = foodkind;
                HomeOrRestaurant = homerestaurant;
                LunchTime = lunchtime;
            }

        }
}
