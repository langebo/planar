using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using Shared.Grpc.Suggestions;
using Suggestions.Data;
using Suggestions.Extensions;
using Suggestions.Models;

namespace Suggestions.Endpoints
{
    public class SuggestionsEndpoint : SuggestionsService.SuggestionsServiceBase
    {
        private readonly SuggestionsContext db;

        public SuggestionsEndpoint(SuggestionsContext db)
        {
            this.db = db;
        }

        public override async Task<SuggestionDto> GetSuggestion(GetSuggestionQueryDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var result = await db.Suggestions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id, context.CancellationToken);

            return result.ToTransfer();
        }

        public override async Task GetSuggestions(GetSuggestionsQueryDto request, IServerStreamWriter<SuggestionDto> responseStream, ServerCallContext context)
        {
            Expression<Func<Suggestion, bool>> filter = s => true;
            switch (request.IdentifierCase)
            {
                case GetSuggestionsQueryDto.IdentifierOneofCase.UserId:
                    var userId = Guid.Parse(request.UserId);
                    filter = s => s.UserId == userId;
                    break;
                case GetSuggestionsQueryDto.IdentifierOneofCase.MeetingId:
                    var meetingId = Guid.Parse(request.MeetingId);
                    filter = s => s.MeetingId == meetingId;
                    break;
                case GetSuggestionsQueryDto.IdentifierOneofCase.None:
                    throw new ArgumentOutOfRangeException("Unrecognized suggestion identifier type (none)");
            }

            var suggestions = db.Suggestions
                .AsNoTracking()
                .Where(filter);

            await foreach (var suggestion in suggestions.AsAsyncEnumerable())
                await responseStream.WriteAsync(suggestion.ToTransfer());
        }

        public override async Task<SuggestionDto> CreateSuggestion(CreateSuggestionCommandDto request, ServerCallContext context)
        {
            var result = await db.Suggestions.AddAsync(new Suggestion
            {
                UserId = Guid.Parse(request.UserId),
                MeetingId = Guid.Parse(request.MeetingId),
                Start = request.Start.ToDateTimeOffset(),
                End = request.End.ToDateTimeOffset()
            }, context.CancellationToken);

            await db.SaveChangesAsync(context.CancellationToken);
            return result.Entity.ToTransfer();
        }

        public override async Task<SuggestionDto> UpdateSuggestion(UpdateSuggestionCommandDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var suggestion = await db.Suggestions.FirstOrDefaultAsync(s => s.Id == id, context.CancellationToken);

            suggestion.UserId = Guid.Parse(request.UserId);
            suggestion.MeetingId = Guid.Parse(request.MeetingId);
            suggestion.Start = request.Start.ToDateTimeOffset();
            suggestion.End = request.End.ToDateTimeOffset();

            var result = db.Suggestions.Attach(suggestion);
            await db.SaveChangesAsync(context.CancellationToken);

            return result.Entity.ToTransfer();
        }

        public override async Task<Empty> DeleteSuggestion(DeleteSuggestionCommandDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var suggestion = await db.Suggestions.FirstOrDefaultAsync(s => s.Id == id, context.CancellationToken);

            db.Suggestions.Remove(suggestion);
            await db.SaveChangesAsync(context.CancellationToken);

            return new Empty();
        }

        public override async Task<SuggestionExistsResponseDto> SuggestionExists(SuggestionExistsQueryDto request, ServerCallContext context)
        {
            var id = Guid.Parse(request.Id);
            var exists = await db.Suggestions
                .AsNoTracking()
                .AnyAsync(s => s.Id == id, context.CancellationToken);

            return new SuggestionExistsResponseDto { Exists = exists };
        }
    }
}