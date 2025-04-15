using System;
using System.Security.Claims;
using System.Threading.Tasks;
using FileDeck.api.DTOs;
using FileDeck.api.Services;
using FileDeck.api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FileDeck.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FolderController : ControllerBase
{
    private readonly IFolderService folderService;
    private readonly IFileService fileService;
    public FolderController(IFolderService folderService, IFileService fileService)
    {
        this.folderService = folderService;
        this.fileService = fileService;
    }

    // Creates a new folder
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateFolderAsync([FromBody] CreateFolderDto folderDto)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not foud in token" });
        }

        Console.WriteLine($"Controller - User ID from token: '{userId}'");

        var newFolder = await folderService.CreateFolderAsync(folderDto, userId);

        // This references the GET method for the Location Header
        return CreatedAtAction(
            nameof(GetFolderById), // Name of the GET method
            new { id = newFolder.Id },  // Route parameters for the GET method
            newFolder                   // The Response body
        );
    }

    // Returns an existing folder
    [HttpGet("{folderId}")]
    [Authorize]
    public async Task<IActionResult> GetFolderById(int folderId)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token" });
        }
        var folder = await folderService.GetFolderByIdAsync(folderId, userId);

        if (folder == null)
        {
            return NotFound();
        }

        return Ok(folder);
    }

    [HttpGet("{folderId}/files")]
    [Authorize]
    public async Task<IActionResult> GetFilesInFolder(int folderId)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token" });
        }

        var files = await fileService.GetFilesInFolderAsync(folderId, userId);

        return Ok(files);
    }

    [HttpPut("{folderId}/rename")]
    [Authorize]
    public async Task<IActionResult> RenameFolderAsync(int folderId, [FromBody] RenameFolderRequest request)
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

        bool success = await folderService.RenameFolderAsync(folderId, request, userId);

        if (success)
        {
            return Ok(new { message = "Folder renamed successfully" });
        }
        else
        {
            return NotFound(new { message = "Folder not found or operation failed" });
        }
    }

    [HttpDelete("{folderId}")]
    [Authorize]
    public async Task<IActionResult> DeleteFolderAsync(int folderId)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        bool success = await folderService.DeleteFolderAsync(folderId, userId);

        if (success)
        {
            return NoContent();
        }
        else
        {
            return NotFound(new { message = "Folder not found or operation failed" });
        }
    }
}