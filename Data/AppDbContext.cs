using Microsoft.EntityFrameworkCore;
using ScheduleCreate.Models;

namespace ScheduleCreate.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Teacher> Teachers { get; set; } = null!;
        public DbSet<Group> Groups { get; set; } = null!;
        public DbSet<Discipline> Disciplines { get; set; } = null!;
        public DbSet<Auditorium> Auditoriums { get; set; } = null!;
        public DbSet<ScheduleEntry> ScheduleEntries { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка связей многие-ко-многим
            modelBuilder.Entity<Teacher>()
                .HasMany(t => t.Disciplines)
                .WithMany(d => d.Teachers);

            modelBuilder.Entity<ScheduleEntry>()
                .HasMany(s => s.Groups)
                .WithMany(g => g.ScheduleEntries);

            // Настройка каскадного удаления
            modelBuilder.Entity<ScheduleEntry>()
                .HasOne(s => s.Teacher)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ScheduleEntry>()
                .HasOne(s => s.Auditorium)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ScheduleEntry>()
                .HasOne(s => s.Discipline)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
} 