using System;

namespace Invites.Models
{
    public class Invite
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid MeetingId { get; set; }
    }
}