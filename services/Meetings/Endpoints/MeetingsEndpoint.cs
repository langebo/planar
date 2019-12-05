using System;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Meetings.Data;
using Meetings.Extensions;
using Meetings.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Grpc.Meetings;
using Shared.Grpc.Users;

namespace Meetings.Endpoints
{
    public class MeetingsEndpoint : MeetingsService.MeetingsServiceBase
    {
        private readonly MeetingsContext db;
        private readonly UsersService.UsersServiceClient usersClient;

        public MeetingsEndpoint(MeetingsContext db, UsersService.UsersServiceClient usersClient)
        {
            this.db = db;
            this.usersClient = usersClient;
        }

        public override async Task<MeetingDto> GetMeeting(GetMeetingQueryDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var meeting = await db.Meetings
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.Id == id, context.CancellationToken);

            return meeting.ToTransfer();
        }

        public override async Task GetMeetings(GetMeetingsQueryDto request, IServerStreamWriter<MeetingDto> responseStream, ServerCallContext context)
        {
            var meetings = db.Meetings.AsNoTracking();
            if (Guid.TryParse(request.UserId, out var userId))
            {
                if (!(await usersClient.UserExistsAsync(new UserExistsQueryDto { Id = request.UserId }, cancellationToken: context.CancellationToken)).Exists)
                    throw new ArgumentException($"User ({request.UserId}) does not exist");

                meetings = meetings.Where(m => m.UserId == userId);
            }

            await foreach (var meeting in meetings.AsAsyncEnumerable())
                await responseStream.WriteAsync(meeting.ToTransfer());
        }

        public override async Task<MeetingDto> CreateMeeting(CreateMeetingCommandDto request, ServerCallContext context)
        {
            var result = await db.Meetings.AddAsync(new Meeting
            {
                UserId = Guid.Parse(request.UserId),
                Start = request.Start.ToDateTimeOffset(),
                End = request.End.ToDateTimeOffset(),
                Name = request.Name,
                Description = request.Descripton
            }, context.CancellationToken);

            await db.SaveChangesAsync(context.CancellationToken);
            return result.Entity.ToTransfer();
        }

        public override async Task<MeetingDto> UpdateMeeting(UpdateMeetingCommandDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var meeting = await db.Meetings
                .SingleOrDefaultAsync(m => m.Id == id, context.CancellationToken);

            meeting.UserId = Guid.Parse(request.UserId);
            meeting.Start = request.Start.ToDateTimeOffset();
            meeting.End = request.End.ToDateTimeOffset();
            meeting.Name = request.Name;
            meeting.Description = request.Descripton;

            var result = db.Meetings.Attach(meeting);
            await db.SaveChangesAsync(context.CancellationToken);

            return result.Entity.ToTransfer();
        }

        public override async Task<Empty> DeleteMeeting(DeleteMeetingCommandDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var meeting = await db.Meetings
                .SingleOrDefaultAsync(m => m.Id == id, context.CancellationToken);

            db.Meetings.Remove(meeting);
            await db.SaveChangesAsync(context.CancellationToken);

            return new Empty();
        }

        public override async Task<MeetingExistsResponseDto> MeetingExists(MeetingExistsQueryDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var exists = await db.Meetings
                .AsNoTracking()
                .AnyAsync(m => m.Id == id, context.CancellationToken);

            return new MeetingExistsResponseDto { Exists = exists };
        }
    }
}