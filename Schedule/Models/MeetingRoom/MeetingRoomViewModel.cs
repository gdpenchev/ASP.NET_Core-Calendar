namespace Schedule.Models.MeetingRoom
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using static Schedule.Data.DataConstants;
    public class MeetingRoomViewModel
    {
        [Required]
        public DateTime CurrentDate { get; set; }
        [Required]
        [Range(minParticipants,maxParticipants, ErrorMessage = "Participants must be between {0} and {1}!")]
        public int Participants { get; set; }
        [Required]
        public int Hours { get; set; }
        [Required]
        public int Minutes { get; set; }
    }
}
