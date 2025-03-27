using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NotesAPI.Models;

namespace NotesAPI.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<NotesUser>(options)
{
    public DbSet<Note> Notes { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Note>()
            .HasOne(e => e.User)
            .WithMany(e => e.Notes)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}