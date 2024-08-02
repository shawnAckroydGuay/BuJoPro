namespace BuJoProApplicationLogic.BuJoCreator
{
    public interface IMonthPlanningCreator
    {
        byte[] CreateSixMonths(int firstMonth, int monthCount = 6);
    }
}