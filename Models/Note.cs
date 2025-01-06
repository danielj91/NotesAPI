namespace NotesAPI.Models;

public class Note
{
    public int Id { get; set; }
    public string UserId { get; set; }
    public NotesUser User { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}