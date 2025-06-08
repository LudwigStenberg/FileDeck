using System.Security.Claims;
using FileDeck.api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileDeck.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{

    private readonly IFileService fileService;
    public FilesController(IFileService fileService)
    {
        this.fileService = fileService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] int? folderId = null)
    {
        if (file == null || file.Length == 0)
        {
            throw new FileEmptyException();
        }

        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var newFile = await fileService.UploadFileAsync(file, folderId, userId);

        return CreatedAtAction(
            nameof(GetFileById),
            new { id = newFile.Id },
            newFile);
    }

    [HttpGet("{id}/download")]
    [Authorize]
    public async Task<IActionResult> DownloadFile(int id)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var file = await fileService.DownloadFileAsync(id, userId);
        return File(file.Content, file.ContentType, file.Name);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetFileById(int id)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var file = await fileService.GetFileByIdAsync(id, userId);
        return Ok(file);
    }

    [HttpGet("root")]
    [Authorize]
    public async Task<IActionResult> GetRootFiles()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        var rootFiles = await fileService.GetRootFilesAsync(userId);
        return Ok(rootFiles);
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeleteFile(int id)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        await fileService.DeleteFileAsync(id, userId);
        return NoContent();
    }
}