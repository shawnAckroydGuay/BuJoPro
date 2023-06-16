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

            var list = monthplanning.Create(2023, 6);
        }
    }
}