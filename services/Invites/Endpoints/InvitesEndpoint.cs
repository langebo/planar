using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Invites.Data;
using Invites.Extensions;
using Invites.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Grpc.Invites;
using Shared.Grpc.Meetings;
using Shared.Grpc.Users;

namespace Invites.Endpoints
{
    public class InvitesEndpoint : InvitesService.InvitesServiceBase
    {
        private readonly InvitesContext db;
        private readonly UsersService.UsersServiceClient usersClient;
        private readonly MeetingsService.MeetingsServiceClient meetingsClient;

        public InvitesEndpoint(InvitesContext db, UsersService.UsersServiceClient usersClient, MeetingsService.MeetingsServiceClient meetingsClient)
        {
            this.db = db;
            this.usersClient = usersClient;
            this.meetingsClient = meetingsClient;
        }

        public override async Task<InviteDto> GetInvite(GetInviteQueryDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var invite = await db.Invites
                .AsNoTracking()
                .SingleOrDefaultAsync(i => i.Id == id, context.CancellationToken);

            return invite.ToTransfer();
        }

        public override async Task GetInvites(GetInvitesQueryDto request, IServerStreamWriter<InviteDto> responseStream, ServerCallContext context)
        {
            Expression<Func<Invite, bool>> filter = i => true;
            switch (request.IdentifierCase)
            {
                case GetInvitesQueryDto.IdentifierOneofCase.UserId:
                    var userId = Guid.Parse(request.UserId);
                    filter = i => i.UserId == userId;
                    break;
                case GetInvitesQueryDto.IdentifierOneofCase.MeetingId:
                    var meetingId = Guid.Parse(request.MeetingId);
                    filter = i => i.MeetingId == meetingId;
                    break;
                case GetInvitesQueryDto.IdentifierOneofCase.None:
                    throw new ArgumentOutOfRangeException("Unrecognized invite identifier type (none)");
            }

            var invites = db.Invites
                .AsNoTracking()
                .Where(filter);

            await foreach (var invite in invites.AsAsyncEnumerable())
                await responseStream.WriteAsync(invite.ToTransfer());
        }

        public override async Task<InviteDto> CreateInvite(CreateInviteCommandDto request, ServerCallContext context)
        {
            var result = await db.Invites.AddAsync(new Invite
            {
                UserId = Guid.Parse(request.UserId),
                MeetingId = Guid.Parse(request.MeetingId)
            }, context.CancellationToken);

            await db.SaveChangesAsync(context.CancellationToken);
            return result.Entity.ToTransfer();
        }

        public override async Task<Empty> DeleteInvite(DeleteInviteCommandDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var invite = await db.Invites
                .SingleOrDefaultAsync(i => i.Id == id, context.CancellationToken);

            db.Invites.Remove(invite);
            await db.SaveChangesAsync(context.CancellationToken);

            return new Empty();
        }

        public override async Task<InviteExistsResponseDto> InviteExists(InviteExistsQueryDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var exists = await db.Invites
                .AsNoTracking()
                .AnyAsync(i => i.Id == id, context.CancellationToken);

            return new InviteExistsResponseDto { Exists = exists };
        }
    }
}