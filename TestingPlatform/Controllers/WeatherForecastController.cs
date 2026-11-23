using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
namespace TestingPlatform.Controllers;

// Учебные упражнения урока 01: route, query и HTTP-заголовки.
[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    public record Forecast(DateOnly Date, int TemperatureC, string Summary);
    private static List<Forecast> Generate(int days) => Enumerable.Range(1, days)
        .Select(i => new Forecast(DateOnly.FromDateTime(DateTime.UtcNow.AddDays(i)),
            Random.Shared.Next(-20, 55), "Учебный прогноз")).ToList();
    [HttpGet]
    public IActionResult Get() => Ok(Generate(5));
    [HttpGet("{days:int}")]
    public IActionResult WithDays([FromRoute, Range(1, 30)] int days) => Ok(Generate(days));
    [HttpGet("filter")]
    public IActionResult Filter([FromQuery] int minTemp = 0, [FromQuery] int maxTemp = 20)
    {
        if (minTemp > maxTemp) return BadRequest(new {message = "minTemp не должен превышать maxTemp."});
        return Ok(Generate(10).Where(f => f.TemperatureC >= minTemp && f.TemperatureC <= maxTemp));
    }
    [HttpGet("check-header")]
    public IActionResult CheckHeader([FromHeader(Name = "X-Client")] string? client)
    {
        Response.Headers["X-Echo-Client"] = client ?? "unknown";
        return Ok(new {message = $"Запрос пришёл от клиента: {client ?? "unknown"}"});
    }
}
