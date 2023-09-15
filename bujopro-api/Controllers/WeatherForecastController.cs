using BuJoProApplicationLogic.BuJoCreator;
using Microsoft.AspNetCore.Mvc;

namespace BuJoProApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMonthPlanningCreator _monthPlanningCreator;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMonthPlanningCreator monthPlanningCreator)
        {
            _logger = logger;
            _monthPlanningCreator = monthPlanningCreator;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost(Name = "GetBuJoPDF")]
        public void CreateBulletJournal()
        {
            _monthPlanningCreator.CreateSixMonths();
        }
    }
}