using FarmManagement.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FarmManagementAPI.FarmManagement.Core.Interfaces.Repositories;
using FarmManagementAPI.FarmManagement.Core.Entities;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FarmRecordsController : ControllerBase
{
    private readonly IFarmRecordRepository _farmRecordRepository;
    private readonly IFarmRepository _farmRepository;

    public FarmRecordsController(IFarmRecordRepository farmRecordRepository, IFarmRepository farmRepository)
    {
        _farmRecordRepository = farmRecordRepository;
        _farmRepository = farmRepository;
    }

    [HttpGet("farm/{farmId}")]
    public async Task<ActionResult<IEnumerable<FarmRecord>>> GetFarmRecords(Guid farmId)
    {
        // Verify farm belongs to user
        var farm = await _farmRepository.GetFarmByIdAsync(farmId);
        if (farm == null || farm.UserId != GetCurrentUserId())
            return NotFound();

        var records = await _farmRecordRepository.GetRecordsByFarmIdAsync(farmId);
        return Ok(records);
    }

    [HttpPost]
    public async Task<ActionResult<FarmRecord>> CreateFarmRecord(FarmRecord record)
    {
        // Verify farm belongs to user
        var farm = await _farmRepository.GetFarmByIdAsync(record.FarmId);
        if (farm == null || farm.UserId != GetCurrentUserId())
            return NotFound();

        record.Id = Guid.NewGuid();
        record.RecordDate = DateTime.UtcNow;

        await _farmRecordRepository.AddFarmRecordAsync(record);
        return CreatedAtAction(nameof(GetFarmRecords), new { farmId = record.FarmId }, record);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateFarmRecord(Guid id, FarmRecord record)
    {
        if (id != record.Id)
            return BadRequest();

        // Verify record belongs to user
        var existingRecord = await GetRecordWithFarmCheck(id);
        if (existingRecord == null)
            return NotFound();

        await _farmRecordRepository.UpdateFarmRecordAsync(record);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFarmRecord(Guid id)
    {
        var record = await GetRecordWithFarmCheck(id);
        if (record == null)
            return NotFound();

        await _farmRecordRepository.DeleteFarmRecordAsync(id);
        return NoContent();
    }

    private async Task<FarmRecord?> GetRecordWithFarmCheck(Guid recordId)
    {
        var record = await _farmRecordRepository.GetRecordByIdAsync(recordId);
        if (record == null) return null;

        var farm = await _farmRepository.GetFarmByIdAsync(record.FarmId);
        if (farm == null || farm.UserId != GetCurrentUserId()) return null;

        return record;
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.Parse(userId!);
    }
}