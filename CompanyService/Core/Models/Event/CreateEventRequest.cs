using CompanyService.Core.Enums;
using System.Text.Json.Serialization;

namespace CompanyService.Core.Models.Event
{

    public class CreateEventRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool AllDay { get; set; } = false;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EventPriority Priority { get; set; } = EventPriority.Low;
        public List<Guid> AttendeeUserIds { get; set; } = new();
    }
}
