using Microsoft.EntityFrameworkCore;
namespace MeetUp.Models
{
    public class MeetUpContext : DbContext 
    { 
        public MeetUpContext(DbContextOptions options) : base(options) { }

        // ================================== User Table
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<ParticipantAtEvent> ParticipantsAtEvents { get; set; }
    }
}