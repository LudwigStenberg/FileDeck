using System.Security.Claims;
using System.Threading.Tasks;
using FileDeck.api.DTOs;
using FileDeck.api.Repositories;
using FileDeck.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace FileDeck.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{

    private readonly IFileService fileService;
    public FileController(IFileService fileService)
    {
        this.fileService = fileService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UploadFileAsync(FileUploadDto fileUpload)
    {
        // Is it a good idea to use this here? 
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token" });
        }

        var newFile = await fileService.UploadFileAsync(fileUpload, userId);

        return CreatedAtAction(
            nameof(GetFileById),
            new { id = newFile.Id },
            newFile);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetFileById(int id)
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token" });
        }

        var file = await fileService.GetFileByIdAsync(id, userId);

        if (file == null)
        {
            return NotFound();
        }

        return Ok(file);
    }

    [HttpGet("{id}/download")]
    [Authorize]
    public async Task<IActionResult> DownloadFileAsync(int id)
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token" });
        }

        var file = await fileService.DownloadFileAsync(id, userId);

        if (file == null)
        {
            return NotFound();
        }

        return File(file.Content, file.ContentType, file.Name);
    }

    [HttpDelete("{fileId}")]
    [Authorize]
    public async Task<IActionResult> DeleteFileAsync(int fileId)
    {
        string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token" });
        }

        var result = await fileService.DeleteFileAsync(fileId, userId);

        if (!result)
        {
            NotFound(result);
        }

        return NoContent();
    }
}