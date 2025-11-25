using Microsoft.AspNetCore.Mvc;
using UrlShortener.WebApi.Services.Interfaces;

namespace UrlShortener.WebApi.Controllers;

public class HomeController(
    ILogger<HomeController> logger,
    IUrlShortenerService shortenerService) : ControllerBase
{
    [HttpGet("/{shortUrl}")]
    public async Task<IActionResult> Index(string shortUrl)
    {
        try
        {
            var longUrl = await shortenerService.GetLongUrlAsync(shortUrl);
            if (longUrl is not null) 
                return Redirect(longUrl.ToString());
            return NotFound();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred");
            return StatusCode(500, "An internal server error occurred.");
        }
    }

    [HttpPost("api/shorten")]
    public async Task<IActionResult> Shorten([FromBody] string url)
    {
        try
        {
            var newUrl = await shortenerService.ShortenUrlAsync(url);
            return Created(newUrl, newUrl);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred");
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}