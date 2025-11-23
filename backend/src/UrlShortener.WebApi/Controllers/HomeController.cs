using Microsoft.AspNetCore.Mvc;
using UrlShortener.WebApi.Services.Interfaces;

namespace UrlShortener.WebApi.Controllers;

public class HomeController(
    ILogger<HomeController> logger,
    IUrlShortenerService shortenerService) : ControllerBase
{
    [HttpGet("/{shortUrl}")]
    public IActionResult Index(string shortUrl)
    {
        try
        {
            var longUrl = shortenerService.GetLongUrl(shortUrl);
            if (longUrl is not null) 
                return Redirect(longUrl.ToString());
            return NotFound();
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured");
            return StatusCode(500, "An internal server error occurred.");
        }
    }

    [HttpPost("api/shorten")]
    public IActionResult Shorten([FromBody] string url)
    {
        try
        {
            var newUrl = shortenerService.ShortenUrl(url);
            return Created(newUrl, newUrl);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured");
            return StatusCode(500, "An internal server error occurred.");
        }
    }
}