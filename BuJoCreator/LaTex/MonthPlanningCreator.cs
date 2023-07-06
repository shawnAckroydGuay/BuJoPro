using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuJoCreator.LaTex
{
    public class MonthPlanningCreator
    {
        public string TemplatePath = "/Users/shawn/Code/productivityAgendaCreator/BuJoCreator/LaTex/monthPlanning.tex";

        public MonthPlanningCreator()
        {

        }

        public string CreateMonthPlanningPage(int year, int month)
        {
            var dayList = CreateDaysList(year, month);
            var dayCount = DateTime.DaysInMonth(year, month);
            return GetMonthPageWithReplacedValues(dayList, dayCount);
        }

        public string CreateDaysList(int year, int month)
        {
            var daysOfTheMonth = "\\def\\bulletCount{";
            var daysOfTheMonths = DateTime.DaysInMonth(year, month);
            for (int i = 1; i <= daysOfTheMonths; i++)
            {
                var day = new DateTime(year, month, i);
                var dayName = day.ToString("D", System.Globalization.CultureInfo.CreateSpecificCulture("fr-FR"));
                daysOfTheMonth += i.ToString("D2") + dayName.Substring(0, 1).ToUpper() + ",";
            }
            for (int i = daysOfTheMonths; i <= 40; i++)
            {
                daysOfTheMonth += "XX,";
            }

            return daysOfTheMonth.Remove(daysOfTheMonth.Length - 1) + "}";

        }

        public string GetMonthPageWithReplacedValues(string dayList, int daysCount)
        {
            string originalMonthPlanningTemplate = File.ReadAllText(TemplatePath);
            var monthPlanningWithDaysList = originalMonthPlanningTemplate.Replace("listOfDaysToReplace", dayList);
            var monthPlanningWithDaysCount = monthPlanningWithDaysList.Replace("daysCountToReplace", daysCount.ToString()).Replace("daysCountPlusOneToReplace", (daysCount + 1).ToString());
            return monthPlanningWithDaysCount;
        }

    }
}
