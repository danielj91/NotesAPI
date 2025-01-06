using Microsoft.AspNetCore.Identity;

namespace NotesAPI.Models;

public class NotesUser : IdentityUser
{
    public ICollection<Note> Notes { get; } = new List<Note>();
}