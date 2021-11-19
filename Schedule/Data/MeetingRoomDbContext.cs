namespace Schedule.Data
{
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;
    public class MeetingRoomDbContext : IdentityDbContext
    {
        public MeetingRoomDbContext(DbContextOptions<MeetingRoomDbContext> options)
            : base(options)
        {
        }
    }
}
