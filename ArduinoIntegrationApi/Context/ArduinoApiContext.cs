using ArduinoIntegrationApi.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ArduinoIntegrationApi.Context
{
    public class ArduinoApiContext : DbContext
    {
        public DbSet<RoomReading> RoomReading { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<JwtToken> JwtTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                "Server=192.168.2.129;Database=ArduinoApiDb;User ID=admdb; Password=Kode1234!;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoomReading>()
                .HasKey(rd => new {rd.Rr_RoomName, rd.Rr_Cts});

        }   
    }
}