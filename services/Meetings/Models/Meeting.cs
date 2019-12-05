using System;

namespace Meetings.Models
{
    public class Meeting
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}