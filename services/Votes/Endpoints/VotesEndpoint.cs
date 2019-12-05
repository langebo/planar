using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Shared.Grpc.Votes;
using Votes.Data;
using Votes.Extensions;
using Votes.Models;

namespace Votes.Endpoints
{
    public class VotesEndpoint : VotesService.VotesServiceBase
    {
        private readonly VotesContext db;

        public VotesEndpoint(VotesContext db)
        {
            this.db = db;
        }

        public override async Task<VoteDto> GetVote(GetVoteQueryDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var result = await db.Votes
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id, context.CancellationToken);

            return result.ToTransfer();
        }

        public override async Task GetVotes(GetVotesQueryDto request, IServerStreamWriter<VoteDto> responseStream, ServerCallContext context)
        {
            Expression<Func<Vote, bool>> filter = v => true;
            switch (request.IdentifierCase)
            {
                case GetVotesQueryDto.IdentifierOneofCase.UserId:
                    var userId = Guid.Parse(request.UserId);
                    filter = v => v.UserId == userId;
                    break;
                case GetVotesQueryDto.IdentifierOneofCase.SuggestionId:
                    var suggestionId = Guid.Parse(request.SuggestionId);
                    filter = v => v.SuggestionId == suggestionId;
                    break;
                case GetVotesQueryDto.IdentifierOneofCase.None:
                    throw new ArgumentOutOfRangeException($"Unrecognized suggestion identifier type (none)");
            }

            var votes = db.Votes
                .AsNoTracking()
                .Where(filter);

            await foreach (var vote in votes.AsAsyncEnumerable())
                await responseStream.WriteAsync(vote.ToTransfer());
        }

        public override async Task<VoteDto> CreateVote(CreateVoteCommandDto request, ServerCallContext context)
        {
            var result = await db.Votes.AddAsync(new Vote
            {
                UserId = Guid.Parse(request.UserId),
                SuggestionId = Guid.Parse(request.SuggestionId)
            }, context.CancellationToken);

            await db.SaveChangesAsync(context.CancellationToken);
            return result.Entity.ToTransfer();
        }

        public override async Task<VoteDto> UpdateVote(UpdateVoteCommandDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var vote = await db.Votes.FirstOrDefaultAsync(v => v.Id == id, context.CancellationToken);

            vote.UserId = Guid.Parse(request.UserId);
            vote.SuggestionId = Guid.Parse(request.SuggestionId);

            var result = db.Votes.Attach(vote);
            await db.SaveChangesAsync(context.CancellationToken);

            return result.Entity.ToTransfer();
        }

        public override async Task<Empty> DeleteVote(DeleteVoteCommandDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var vote = await db.Votes.FirstOrDefaultAsync(v => v.Id == id, context.CancellationToken);

            db.Votes.Remove(vote);
            await db.SaveChangesAsync(context.CancellationToken);

            return new Empty();
        }

        public override async Task<VoteExistsResponseDto> VoteExists(VoteExistsQueryDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var exists = await db.Votes
                .AsNoTracking()
                .AnyAsync(v => v.Id == id, context.CancellationToken);

            return new VoteExistsResponseDto { Exists = exists };
        }
    }
}