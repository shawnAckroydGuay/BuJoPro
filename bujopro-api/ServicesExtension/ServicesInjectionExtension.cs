using BuJoProApplicationLogic.BuJoCreator;

namespace bujopro_api.ServicesExtension
{
    internal static class ServicesInjectionExtension
    {
        internal static void ConfigureDepedencyInjectionServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IMonthPlanningCreator, MonthPlanningCreator>();
        }
    }
}
