using System.Security.Claims;
using System.Threading.Tasks;
using FileDeck.api.DTOs;
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
    public async Task<IActionResult> UploadFile(FileUploadRequest fileUpload)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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

    [HttpGet("{id}/download")]
    [Authorize]
    public async Task<IActionResult> DownloadFile(int id)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetFileById(int id)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

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

    [HttpGet("root")]
    [Authorize]
    public async Task<IActionResult> GetRootFiles()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token" });
        }

        var rootFiles = await fileService.GetRootFilesAsync(userId);

        return Ok(rootFiles);
    }



    [HttpDelete("{fileId}")]
    [Authorize]
    public async Task<IActionResult> DeleteFile(int fileId)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token" });
        }

        var result = await fileService.DeleteFileAsync(fileId, userId);

        if (!result)
        {
            return NotFound();
        }

        return NoContent();
    }
}