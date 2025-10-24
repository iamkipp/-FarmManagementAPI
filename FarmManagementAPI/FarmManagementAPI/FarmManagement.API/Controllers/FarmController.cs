using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FarmController : ControllerBase
{
    private readonly IFarmRepository _farmRepository;

    public FarmsController(IFarmRepository farmRepository)
    {
        _farmRepository = farmRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Farm>>> GetFarms()
    {
        var userId = GetCurrentUserId();
        var farms = await _farmRepository.GetFarmsByUserIdAsync(userId);
        return Ok(farms);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Farm>> GetFarm(Guid id)
    {
        var farm = await _farmRepository.GetFarmByIdAsync(id);
        if (farm == null || farm.UserId != GetCurrentUserId())
            return NotFound();

        return Ok(farm);
    }

    [HttpPost]
    public async Task<ActionResult<Farm>> CreateFarm(Farm farm)
    {
        farm.Id = Guid.NewGuid();
        farm.UserId = GetCurrentUserId();
        farm.CreatedAt = DateTime.UtcNow;

        await _farmRepository.AddFarmAsync(farm);
        return CreatedAtAction(nameof(GetFarm), new { id = farm.Id }, farm);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFarm(Guid id, Farm farm)
    {
        if (id != farm.Id)
            return BadRequest();

        var existingFarm = await _farmRepository.GetFarmByIdAsync(id);
        if (existingFarm == null || existingFarm.UserId != GetCurrentUserId())
            return NotFound();

        await _farmRepository.UpdateFarmAsync(farm);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFarm(Guid id)
    {
        var farm = await _farmRepository.GetFarmByIdAsync(id);
        if (farm == null || farm.UserId != GetCurrentUserId())
            return NotFound();

        await _farmRepository.DeleteFarmAsync(id);
        return NoContent();
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userId!);
    }
}