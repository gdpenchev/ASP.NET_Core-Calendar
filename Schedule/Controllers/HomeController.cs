namespace Schedule.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Schedule.Data.Models;
    using Schedule.Models;
    using Schedule.Models.MeetingRoom;
    using System;
    using System.Net;
    using System.Linq;
    using System.Diagnostics;
    using System.Collections.Generic;
    
    public class HomeController : Controller
    {
        public IActionResult Index()
            => View();

        [HttpPost]
        public IActionResult Index(MeetingRoomViewModel inputRoom)
        {
            var meetingRooms = GetMeetingRooms(@"../Schedule/json.json");
            var dict = new Dictionary<string, List<Schedules>>();

            if (!ModelState.IsValid)
            {
                return View(inputRoom);
            }

            for (int i = 0; i < meetingRooms.Count; i++)
            {
                var currentRoom = meetingRooms[i];
                var availableTimeSlots = new List<Schedules>();
                if (currentRoom.Capacity < inputRoom.Participants)
                {
                    continue;
                }
                var currentRoomavailableFrom = currentRoom.AvailableFrom;
                var startTimeHour = int.Parse(currentRoomavailableFrom.Split(":")[0]);
                var startTimemMinutes = int.Parse(currentRoomavailableFrom.Split(":")[1]);

                var availableTo = currentRoom.AvailableTo;
                var endTimeHour = int.Parse(availableTo.Split(":")[0]);
                var endTimeMinutes = int.Parse(availableTo.Split(":")[1]);

                var currentRoomSchedule = currentRoom.Schedule.ToList();

                DateTime meetingRoomStart = new DateTime(inputRoom.CurrentDate.Year, inputRoom.CurrentDate.Month, inputRoom.CurrentDate.Day, startTimeHour, startTimemMinutes, 0);
                DateTime meetingRoomEnd = new DateTime(inputRoom.CurrentDate.Year, inputRoom.CurrentDate.Month, inputRoom.CurrentDate.Day, endTimeHour, endTimeMinutes, 0);

                DateTime timeSlotsStartTime = meetingRoomStart;
                DateTime timeSlotsEndTime = timeSlotsStartTime.AddHours(inputRoom.Hours).AddMinutes(inputRoom.Minutes);

                var step = 15;
                //while timeslots are in the working time of the room we check if the room schedule is available
                while (meetingRoomEnd.CompareTo(timeSlotsEndTime) >= 0)
                {
                    var startFrom = new DateTime();
                    bool isTimeSlotBooked = false;
                    for (int k = 0; k < currentRoomSchedule.Count; k++)
                    {
                        var currentSchedule = currentRoomSchedule[k];

                        if ((timeSlotsStartTime >= currentSchedule.From && timeSlotsStartTime < currentSchedule.To) || (timeSlotsEndTime > currentSchedule.From && timeSlotsEndTime <= currentSchedule.To))
                        {
                            isTimeSlotBooked = true;
                            startFrom = currentSchedule.To;
                            break;
                        }
                    }
                    if (isTimeSlotBooked)
                    {
                        timeSlotsStartTime = startFrom;
                        timeSlotsEndTime = startFrom.AddHours(inputRoom.Hours).AddMinutes(inputRoom.Minutes);
                        continue;
                    }
                    else
                    {
                        availableTimeSlots.Add(new Schedules
                        {
                            From = timeSlotsStartTime,
                            To = timeSlotsEndTime
                        });
                        timeSlotsStartTime = timeSlotsStartTime.AddMinutes(step);
                        timeSlotsEndTime = timeSlotsEndTime.AddMinutes(step);
                    }

                }
                if (availableTimeSlots.Count != 0)
                {
                    dict.Add(currentRoom.RoomName, availableTimeSlots);
                }
                
            }
            TempData["dict"] = JsonConvert.SerializeObject(dict);
            return RedirectToAction("Choose", "Home");
        }
        public IActionResult Choose([FromQuery] MeetingRoomChooseAvailableSlotViewModel query)
        {
            var dict = new Dictionary<string, List<Schedules>>();
            if (TempData.ContainsKey("dict"))
            {
                dict = JsonConvert.DeserializeObject<Dictionary<string, List<Schedules>>>((string)TempData["dict"]);
            }
            
            TempData["dict"] = JsonConvert.SerializeObject(dict);

            if (query.ChosenSchedule != null)
            {
                var meetingRooms = GetMeetingRooms(@"../Schedule/json.json");

                string roomName = query.ChosenSchedule.Split("-")[0];
                string fullDateFrom = query.ChosenSchedule.Split("-")[1];
                string fullDateTo = query.ChosenSchedule.Split("-")[2];

                var dateFrom = fullDateFrom.Split(" ")[0];
                var timeFrom = fullDateFrom.Split(" ")[1];

                var dateTo = fullDateTo.Split(" ")[0];
                var timeTo = fullDateTo.Split(" ")[1];

                var dayFrom = int.Parse(dateFrom.Split("/")[0]);
                var monthFrom = int.Parse(dateFrom.Split("/")[1]);
                var yearFrom = int.Parse(dateFrom.Split("/")[2]);

                var dayTo = int.Parse(dateTo.Split("/")[0]);
                var monthTo = int.Parse(dateTo.Split("/")[1]);
                var yearTo = int.Parse(dateTo.Split("/")[2]);

                var hourFrom = int.Parse(timeFrom.Split(":")[0]);
                var minutesFrom = int.Parse(timeFrom.Split(":")[1]);

                var hourTo = int.Parse(timeTo.Split(":")[0]);
                var minutesTo = int.Parse(timeTo.Split(":")[1]);

                DateTime newSceduleFrom = new DateTime(yearFrom, monthFrom, dayFrom, hourFrom, minutesFrom, 0);
                DateTime newSceduleTo = new DateTime(yearTo, monthTo, dayTo, hourTo, minutesTo, 0);
                var schedule = new Schedules
                {
                    From = newSceduleFrom,
                    To = newSceduleTo
                };
                meetingRooms.Find(mr => mr.RoomName == roomName).Schedule.Add(schedule);
                UpdateMeetingRoomsJson(meetingRooms, @"../Schedule/json.json");
                return RedirectToAction("Index");
            }
            return View(new MeetingRoomChooseAvailableSlotViewModel
            {
                AvailableSlots = dict,
                Name = query.Name
            });
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public List<MeetingRoom> GetMeetingRooms(string path)
        {
            var webClient = new WebClient();
            var json = webClient.DownloadString(path);
            List<MeetingRoom> meetingRooms = JsonConvert.DeserializeObject<List<MeetingRoom>>(json);
            return meetingRooms;
        }
        public void UpdateMeetingRoomsJson(List<MeetingRoom> meetingRooms, string path)
        {
            var webClient = new WebClient();
            var jsonOutput = JsonConvert.SerializeObject(meetingRooms, Formatting.Indented);
            webClient.UploadString(path, jsonOutput);
        }

    }
}
