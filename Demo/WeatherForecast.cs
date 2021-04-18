using System;
using DTOGenerator.Attributes;

namespace Demo
{
    [GenerateDto("WeatherForecastDTO", "TestingWeather")]
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        [ExcludeProperty]
        public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);

        [ExcludeProperty("TestingWeather")]
        public string Summary { get; set; }

        [UseExistingDto("TestingWeather > StationWithNoNameDTO")]
        public Station Station { get; set; }
    }
}