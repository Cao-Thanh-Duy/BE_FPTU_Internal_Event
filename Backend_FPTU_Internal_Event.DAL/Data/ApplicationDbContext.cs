using Backend_FPTU_Internal_Event.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend_FPTU_Internal_Event.DAL.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Speaker> Speakers { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Slot> Slots { get; set; }
        public DbSet<StaffEvent> StaffEvents { get; set; }
        public DbSet<SpeakerEvent> SpeakerEvents { get; set; }
        public DbSet<EventSchedule> EventSchedules { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId);
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.Property(e => e.HashPassword).IsRequired();

                entity.HasOne(e => e.Role)
                    .WithMany()
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Role
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.RoleId);
                entity.Property(e => e.RoleName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.RoleDescription).HasMaxLength(255);
            });

            // Event
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.EventId);
                entity.Property(e => e.EventName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.EventDescription).HasMaxLength(1000);
                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Venue)
                    .WithMany()
                    .HasForeignKey(e => e.VenueId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Ticket
            modelBuilder.Entity<Ticket>(entity =>
            {
                entity.HasKey(e => e.TicketId);
                entity.Property(e => e.Status).HasMaxLength(50);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Event)
                    .WithMany()
                    .HasForeignKey(e => e.EventId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Speaker
            modelBuilder.Entity<Speaker>(entity =>
            {
                entity.HasKey(e => e.SpeakerId);
                entity.Property(e => e.SpeakerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SpeakerDescription).HasMaxLength(500);
            });

            // Venue
            modelBuilder.Entity<Venue>(entity =>
            {
                entity.HasKey(e => e.VenueId);
                entity.Property(e => e.VenueName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.LocationDetails).HasMaxLength(500);
            });

            // Slot
            modelBuilder.Entity<Slot>(entity =>
            {
                entity.HasKey(e => e.SlotId);
                entity.Property(e => e.SlotName).IsRequired().HasMaxLength(100);
            });

            // StaffEvent (Many-to-Many)
            modelBuilder.Entity<StaffEvent>(entity =>
            {
                entity.HasKey(e => new { e.EventId, e.UserId });

                entity.HasOne(e => e.Event)
                    .WithMany()
                    .HasForeignKey(e => e.EventId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // SpeakerEvent (Many-to-Many)
            modelBuilder.Entity<SpeakerEvent>(entity =>
            {
                entity.HasKey(e => new { e.SpeakerId, e.EventId });

                entity.HasOne<Speaker>()
                    .WithMany()
                    .HasForeignKey(e => e.SpeakerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Event>()
                    .WithMany()
                    .HasForeignKey(e => e.EventId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // EventSchedule (Many-to-Many)
            modelBuilder.Entity<EventSchedule>(entity =>
            {
                entity.HasKey(e => new { e.SlotId, e.EventId });

                entity.HasOne(e => e.Slot)
                    .WithMany()
                    .HasForeignKey(e => e.SlotId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Event)
                    .WithMany()
                    .HasForeignKey(e => e.EventId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
