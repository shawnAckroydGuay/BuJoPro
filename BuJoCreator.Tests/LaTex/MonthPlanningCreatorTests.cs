using BuJoCreator.LaTex;

namespace BuJoCreator.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var monthplanning = new MonthPlanningCreator();

            var list = monthplanning.CreateDaysList(2023, 6);
        }

        [Test]
        public void Test2()
        {
            var monthplanning = new MonthPlanningCreator();

            var page = monthplanning.CreateMonthPlanningPage(2023, 6);
            File.WriteAllText("/Users/shawn/Code/productivityAgendaCreator/BuJoCreator/LaTex/monthPlanningTest.tex", page);
        }
    }
}