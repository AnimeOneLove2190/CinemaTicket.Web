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
