using System;
using Invites.Models;
using Shared.Grpc.Invites;

namespace Invites.Extensions
{
    public static class InviteExtensions
    {
        public static InviteDto ToTransfer(this Invite invite) =>
            new InviteDto
            {
                Id = $"{invite.Id}",
                UserId = $"{invite.UserId}",
                MeetingId = $"{invite.MeetingId}"
            };

        public static Invite ToModel(this InviteDto invite) =>
            new Invite
            {
                Id = Guid.Parse(invite.Id),
                UserId = Guid.Parse(invite.UserId),
                MeetingId = Guid.Parse(invite.MeetingId)
            };
    }
}