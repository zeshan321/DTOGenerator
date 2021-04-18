using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SourceDto;

namespace Demo.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecastDTO> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)],
                    Station = new Station()
                    {
                        Name = rng.Next(-1000, 1000).ToString(),
                        Level = rng.Next(-20, 55)
                    }
                })
                .Select(s => new WeatherForecastDTO().Map(s))
                .ToArray();
        }
        
        [HttpGet("test2")]
        public IEnumerable<TestingWeather> GetDynamic()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateTime.Now.AddDays(index),
                    TemperatureC = rng.Next(-20, 55),
                    Summary = Summaries[rng.Next(Summaries.Length)],
                    Station = new Station()
                    {
                        Name = rng.Next(-1000, 1000).ToString(),
                        Level = rng.Next(-20, 55)
                    }
                })
                .Select(s => new TestingWeather().Map(s))
                //.Select(s => new WeatherForecastDTO().Map(s))
                .ToArray();
        }
    }
}