using Microsoft.AspNetCore.Mvc;
using TrackItAPI.Database.Repositories;
using TrackItCommon;

namespace TrackItAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class TrackersController : ControllerBase
{

    private readonly ILogger<TrackersController> _logger;
    private readonly ITrackingRepository _trackingRepository;

    public TrackersController(ILogger<TrackersController> logger, ITrackingRepository trackingRepository)
    {
        _logger = logger;
        _trackingRepository = trackingRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _trackingRepository.GetTrackersAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Tracker tracker)
    {
        if (tracker == null)
        {
            return BadRequest();
        }

        await _trackingRepository.AddTrackerAsync(tracker);
        return Ok();
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> Get(string code)
    {
        return Ok(await _trackingRepository.GetTrackerAsync(code));
    }
}
