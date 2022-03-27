using Microsoft.AspNetCore.Mvc;
using TrackItAPI.Database.Repositories;
using TrackItCommon;

namespace TrackItAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class PositionsController : ControllerBase
{
    private readonly ILogger<PositionsController> _logger;
    private readonly ITrackingRepository _trackingRepository;

    public PositionsController(ILogger<PositionsController> logger, ITrackingRepository trackingRepository)
    {
        _logger = logger;
        _trackingRepository = trackingRepository;
    }

    [HttpGet("{code}/last/{count}/startfrom/{offset}")]
    public async Task<IActionResult> Get(string code, int count = 10, int offset = 0, [FromQuery] DateTime? from = null)
    {
        return Ok(await _trackingRepository.GetLastPositionsAsync(code, count, offset, from ?? DateTime.UtcNow));
    }
}