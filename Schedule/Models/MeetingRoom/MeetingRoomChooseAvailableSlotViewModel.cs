namespace Schedule.Models.MeetingRoom
{
    using Schedule.Data.Models;
    using System.Collections.Generic;
    public class MeetingRoomChooseAvailableSlotViewModel
    {
        public Dictionary<string, List<Schedules>> AvailableSlots { get; set; }

        public string Name { get; set; }

        public string ChosenSchedule { get; set; }
    }
}
