using CompanyService.Core.Enums;
using System.Text.Json.Serialization;

namespace CompanyService.Core.Models.Event
{
    public class EventDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool AllDay { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EventPriority Priority { get; set; }
        public bool IsActive { get; set; }
        public List<EventAttendeeDto> Attendees { get; set; } = new();
    }

}
