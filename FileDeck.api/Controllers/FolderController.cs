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

    // Create Folder
    [HttpPost]
    public async Task<IActionResult> CreateFolderAsync([FromBody] CreateFolderDto folderDto)
    {
        return null; // temporary
    }

    // Retrieve Folder
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFolderByIdAsync(int id)
    {
        return null;
    }
}