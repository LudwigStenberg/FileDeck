

using System.ComponentModel.DataAnnotations;

namespace FileDeck.api.DTOs;
public class RenameFolderRequest
{
    [Required]
    [StringLength(50)]
    public required string NewName { get; set; }
}