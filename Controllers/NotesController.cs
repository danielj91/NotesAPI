using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesAPI.Database;
using NotesAPI.Models;

namespace NotesAPI.Controllers;

[Route("[controller]")]
[ApiController]
[Authorize]
public class NotesController : ControllerBase
{
    readonly AppDbContext _dbContext;

    public NotesController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public ActionResult<IEnumerable<NoteDto>> GetAllNotes()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var notes = _dbContext.Notes
            .Where(n => n.UserId == userId)
            .Select(n => NoteToDto(n))
            .ToList();

        return Ok(notes);
    }

    [HttpGet("{id}")]
    public ActionResult<NoteDto> GetNote(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var note = _dbContext.Notes.Find(id);

        if (note == null)
        {
            return NotFound();
        }

        if (note.UserId != userId)
        {
            NotFound();
        }

        return NoteToDto(note);
    }

    [HttpPost]
    public IActionResult CreateNote(NoteDto noteDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var note = new Note
        {
            Id = noteDto.Id,
            Title = noteDto.Title,
            Content = noteDto.Content
        };

        note.UserId = userId;

        _dbContext.Notes.Add(note);
        _dbContext.SaveChanges();

        return CreatedAtAction(nameof(GetNote), new { id = note.Id }, NoteToDto(note));
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteNote(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var note = _dbContext.Notes.Find(id);

        if (note == null)
        {
            return NotFound();
        }

        if (userId != note.UserId)
        {
            return NotFound();
        }

        _dbContext.Notes.Remove(note);
        _dbContext.SaveChanges();

        return NoContent();
    }

    [HttpPut("{id}")]
    public IActionResult UpdateNote(int id, NoteDto noteDto)
    {
        if (id != noteDto.Id)
        {
            return BadRequest();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var note = _dbContext.Notes.Find(noteDto.Id);

        if (note == null)
        {
            return NotFound();
        }

        if (userId != note.UserId)
        {
            return NotFound();
        }

        note.Title = noteDto.Title;
        note.Content = noteDto.Content;

        try
        {
            _dbContext.SaveChanges();
        }
        catch (DbUpdateConcurrencyException) when (!NoteExists(id))
        {
            return NotFound();
        }

        return NoContent();
    }

    bool NoteExists(int id)
    {
        return _dbContext.Notes.Any(n => n.Id == id);
    }

    static NoteDto NoteToDto(Note note) =>
    new NoteDto
    {
        Id = note.Id,
        Title = note.Title,
        Content = note.Content
    };
}
