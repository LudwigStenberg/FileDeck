using System.Threading.Tasks;
using FileDeck.api.DTOs;
using FileDeck.api.Repositories.Interfaces;
using FileDeck.api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace FileDeck.api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FolderController : ControllerBase
{
    private readonly IFolderService folderService;
    public FolderController(IFolderService folderService)
    {
        this.folderService = folderService;
    }

    // Creates a new folder
    [HttpPost]
    public async Task<IActionResult> CreateFolderAsync([FromBody] CreateFolderDto folderDto)
    {
        var newFolder = await folderService.CreateFolderAsync(folderDto, "userId");

        // This references the GET method for the Location Header
        return CreatedAtAction(
            nameof(GetFolderByIdAsync), // Name of the GET method
            new { id = newFolder.Id },  // Route parameters for the GET method
            newFolder                   // The Response body
        );
    }

    // Returns an existing folder
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFolderByIdAsync(int id)
    {
        var folder = await folderService.GetFolderByIdAsync(id);

        if (folder == null)
        {
            return NotFound();
        }

        return Ok(folder);
    }
}