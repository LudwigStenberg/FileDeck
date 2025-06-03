using System.Security.Claims;
using FileDeck.api.DTOs;
using FileDeck.api.Services;
using FileDeck.api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FileDeck.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FoldersController : ControllerBase
{
    private readonly IFolderService folderService;
    private readonly IFileService fileService;
    public FoldersController(IFolderService folderService, IFileService fileService)
    {
        this.folderService = folderService;
        this.fileService = fileService;
    }

    // Creates a new folder
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateFolder([FromBody] CreateFolderRequest request)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token" });
        }

        var newFolder = await folderService.CreateFolderAsync(request, userId);

        return CreatedAtAction(
            nameof(GetFolderById),
            new { folderId = newFolder.Id },
            newFolder
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

        return Ok(folder);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllFolders()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token" });
        }

        var folders = await folderService.GetAllFoldersAsync(userId);

        return Ok(folders);
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

    // Potentially Deprecated (GetAllFolders)
    [HttpGet("{folderId}/subfolders")]
    [Authorize]
    public async Task<IActionResult> GetSubfolders(int folderId)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token" });
        }

        var subfolders = await folderService.GetSubfoldersAsync(folderId, userId);
        return Ok(subfolders);
    }

    // Potentially Deprecated (GetAllFolders)
    [HttpGet("root")]
    [Authorize]
    public async Task<IActionResult> GetRootFolders()
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(new { message = "User ID not found in token" });
        }

        var rootFolders = await folderService.GetRootFoldersAsync(userId);
        return Ok(rootFolders);
    }

    [HttpPut("{folderId}/rename")]
    [Authorize]
    public async Task<IActionResult> RenameFolder(int folderId, [FromBody] RenameFolderRequest request)
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

        await folderService.RenameFolderAsync(folderId, request, userId);

        return NoContent();
    }

    [HttpDelete("{folderId}")]
    [Authorize]
    public async Task<IActionResult> DeleteFolder(int folderId)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        await folderService.DeleteFolderAsync(folderId, userId);

        return NoContent();

    }
}