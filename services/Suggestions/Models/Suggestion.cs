using System;

namespace Suggestions.Models
{
    public class Suggestion
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid MeetingId { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
    }
}