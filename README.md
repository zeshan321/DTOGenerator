# DTOGenerator
Experimental **zero code** compile time DTO generator and mapper using [Source Generators](https://devblogs.microsoft.com/dotnet/introducing-c-source-generators/) in C#.

# Attributes
| Attribute  | Description | Notes |
| :---         | ---------- |---------------- |
| GenerateDto  | Add to class to generate DTO. | If DTO name isn't supplied, by default DTOGenerator will create one called "**className**DTO". DTOGenerator can create unlimited amount of DTOs from the same class. Refer to example below. |
| ExcludeProperty  | Add to exclude property in DTO.  | You can supply a name of a DTO to only exclude in specific ones, else it will be excluded in all DTOs. |
| UseExistingDto  | Add to use an existing DTO for an object that is nested in another DTO.  | You can supply a specific DTO to use based off of.  |

# Example
| Screenshot  | Link | Description |
| ---------------- | --- | ---------- |
| ![Station class](https://i.imgur.com/gKT2xuh.png) | [Link](https://github.com/zeshan321/DTOGenerator/blob/master/Demo/Station.cs) | This will create two DTOs "StationDTO" and "StationWithNoNameDTO". "Name" will be excluded in "StationWithNoNameDTO" and "Level" will be excluded in all. |
| ![WeatherForcast class](https://i.imgur.com/OAIIy5a.png) | [Link](https://github.com/zeshan321/DTOGenerator/blob/master/Demo/WeatherForecast.cs) | This will create two DTOs "WeatherForecastDTO" and "TestingWeather". "TemperatureF" will be excluded in all DTOs. "Summary" will be excluded in the "TestingWeather" DTO. "StationWithNoNameDTO" will be used in "TestingWeather" but in "WeatherForecastDTO" it will use the default "StationDTO". |

# Example of generated classes from above
```csharp
public class StationDTO
{
  public string Name { get; set; }
  
  public StationDTO Map(Station instance)
  {
    Name = instance.Name;
    return this;
   }
}
```
```csharp
public class StationWithNoNameDTO
{
  public StationWithNoNameDTO Map(Station instance)
  {
    return this;
   }
}
```
```csharp
public class WeatherForecastDTO
{
  public DateTime Date { get; set; }
  public int TemperatureC { get; set; }
  public string Summary { get; set; }
  public StationDTO Station { get; set; }
  
  public WeatherForecastDTO Map(WeatherForecast instance)
  {
    Date = instance.Date;
    TemperatureC = instance.TemperatureC;
    Summary = instance.Summary;
    Station = new StationDTO().Map(instance.Station);
    return this;
   }
}
```
