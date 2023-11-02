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
        }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Row> Rows { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
    }
}
