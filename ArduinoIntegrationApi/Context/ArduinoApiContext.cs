using ArduinoIntegrationApi.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ArduinoIntegrationApi.Context
{
    public class ArduinoApiContext : DbContext
    {
        // property to collect / insert data into RoomReading table in the database
        public DbSet<RoomReading> RoomReading { get; set; }
        // property to collect / insert data into Users table in the database
        public DbSet<Users> Users { get; set; }
        // property to collect / insert data into JwtTokens table in the database
        public DbSet<JwtToken> JwtTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // use below server
            optionsBuilder.UseSqlServer(
                "Server=192.168.2.129;Database=ArduinoApiDb;User ID=admdb; Password=Kode1234!;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // set Rr_Roomname and Rr_Cts as a combined primary key
            modelBuilder.Entity<RoomReading>()
                .HasKey(rd => new {rd.Rr_RoomName, rd.Rr_Cts});

        }   
    }
}