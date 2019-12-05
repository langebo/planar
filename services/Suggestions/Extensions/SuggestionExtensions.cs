using System;
using Google.Protobuf.WellKnownTypes;
using Shared.Grpc.Suggestions;
using Suggestions.Models;

namespace Suggestions.Extensions
{
    public static class SuggestionExtensions
    {
        public static SuggestionDto ToTransfer(this Suggestion suggestion) =>
            new SuggestionDto
            {
                Id = $"{suggestion.Id}",
                UserId = $"{suggestion.UserId}",
                MeetingId = $"{suggestion.MeetingId}",
                Start = Timestamp.FromDateTimeOffset(suggestion.Start),
                End = Timestamp.FromDateTimeOffset(suggestion.End)
            };

        public static Suggestion ToModel(this SuggestionDto suggestion) =>
            new Suggestion
            {
                Id = Guid.Parse(suggestion.Id),
                UserId = Guid.Parse(suggestion.UserId),
                MeetingId = Guid.Parse(suggestion.MeetingId),
                Start = suggestion.Start.ToDateTimeOffset(),
                End = suggestion.End.ToDateTimeOffset()
            };
    }
}