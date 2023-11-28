using System;
using Microsoft.EntityFrameworkCore;
using CinemaTicket.Entities;

namespace CinemaTicket.DataAccess
{
    public class CinemaManagerContext : DbContext
    {
        public CinemaManagerContext(DbContextOptions<CinemaManagerContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Session>()
                 .HasMany(s => s.Tickets)
                 .WithOne(t => t.Session)
                 .HasForeignKey(t => t.SessionId)
                 .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasData(new Role { Id = Guid.Parse("3868C13D-8D12-46A6-B709-52BCF010BDFF"), Name = "Admin" });
            modelBuilder.Entity<Role>().HasData(new Role { Id = Guid.Parse("375AF7CC-A281-4432-A3F5-14AF10BF73F6"), Name = "DefaultUser" });
            modelBuilder.Entity<Account>().HasData(new Account { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Login = "System", RoleId = Guid.Parse("3868C13D-8D12-46A6-B709-52BCF010BDFF") });

            modelBuilder.Entity<Account>()
                .HasMany(a => a.CreatedGenres)
                .WithOne(c => c.CreatedByUser)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.ModifiedGenres)
                .WithOne(c => c.ModifiedByUser)
                .HasForeignKey(c => c.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.CreatedHalls)
                .WithOne(c => c.CreatedByUser)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.ModifiedHalls)
                .WithOne(c => c.ModifiedByUser)
                .HasForeignKey(c => c.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.CreatedMovies)
                .WithOne(c => c.CreatedByUser)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.ModifiedMovies)
                .WithOne(c => c.ModifiedByUser)
                .HasForeignKey(c => c.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.CreatedPlaces)
                .WithOne(c => c.CreatedByUser)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.ModifiedPlaces)
                .WithOne(c => c.ModifiedByUser)
                .HasForeignKey(c => c.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.CreatedRows)
                .WithOne(c => c.CreatedByUser)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.ModifiedRows)
                .WithOne(c => c.ModifiedByUser)
                .HasForeignKey(c => c.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.CreatedSessions)
                .WithOne(c => c.CreatedByUser)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.ModifiedSessions)
                .WithOne(c => c.ModifiedByUser)
                .HasForeignKey(c => c.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.CreatedTickets)
                .WithOne(c => c.CreatedByUser)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.ModifiedTickets)
                .WithOne(c => c.ModifiedByUser)
                .HasForeignKey(c => c.ModifiedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Row> Rows { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Account> Accounts { get; set; }
    }
}
