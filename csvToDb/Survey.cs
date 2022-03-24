using System;
namespace csvToDb
{
        internal class Survey
        {
            public int MeatWeeks { get; set; }
            public string KindOfFood { get; set; }
            public string HomeOrRestaurant { get; set; }
            public int LunchTime { get; set; }

            public Survey(int weeks, string foodkind, string homerestaurant, int lunchtime)
            {
                MeatWeeks = weeks;
                KindOfFood = foodkind;
                HomeOrRestaurant = homerestaurant;
                LunchTime = lunchtime;
            }

        }
}
