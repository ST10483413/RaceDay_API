using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RaceDay_API.Models;

namespace RaceDay_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Organiser> Organisers { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Result> Results { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Unique Email
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // One-to-One: User ↔ Organiser
            modelBuilder.Entity<Organiser>()
                .HasOne(o => o.User).WithOne(u => u.Organiser)
                .HasForeignKey<Organiser>(o => o.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-One: User ↔ Participant
            modelBuilder.Entity<Participant>()
                .HasOne(p => p.User).WithOne(u => u.Participant)
                .HasForeignKey<Participant>(p => p.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Organiser → Events
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organiser).WithMany(o => o.Events)
                .HasForeignKey(e => e.OrganiserID)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Event → Categories
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Event).WithMany(e => e.Categories)
                .HasForeignKey(c => c.EventID)
                .OnDelete(DeleteBehavior.Cascade);

            // Enrollment → Participant (Restrict to avoid cascade path error)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Participant).WithMany(p => p.Enrollments)
                .HasForeignKey(e => e.ParticipantID)
                .OnDelete(DeleteBehavior.Restrict);

            // Enrollment → Category (Cascade)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Category).WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CategoryID)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-One: Enrollment ↔ Result
            modelBuilder.Entity<Result>()
                .HasOne(r => r.Enrollment).WithOne(e => e.Result)
                .HasForeignKey<Result>(r => r.EnrollmentID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}