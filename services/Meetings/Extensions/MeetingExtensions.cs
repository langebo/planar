using System;
using Google.Protobuf.WellKnownTypes;
using Meetings.Models;
using Shared.Grpc.Meetings;

namespace Meetings.Extensions
{
    public static class MeetingExtensions
    {
        public static MeetingDto ToTransfer(this Meeting meeting) =>
            new MeetingDto
            {
                Id = $"{meeting.Id}",
                UserId = $"{meeting.UserId}",
                Start = Timestamp.FromDateTimeOffset(meeting.Start),
                End = Timestamp.FromDateTimeOffset(meeting.End),
                Name = meeting.Name,
                Descripton = meeting.Description
            };

        public static Meeting ToModel(this MeetingDto meeting) =>
            new Meeting
            {
                Id = Guid.Parse(meeting.Id),
                UserId = Guid.Parse(meeting.UserId),
                Start = meeting.Start.ToDateTimeOffset(),
                End = meeting.End.ToDateTimeOffset(),
                Name = meeting.Name,
                Description = meeting.Descripton
            };
    }
}