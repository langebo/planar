using System;
using Shared.Grpc.Votes;
using Votes.Models;

namespace Votes.Extensions
{
    public static class VoteExtensions
    {
        public static VoteDto ToTransfer(this Vote vote) =>
            new VoteDto
            {
                Id = $"{vote.Id}",
                UserId = $"{vote.UserId}",
                SuggestionId = $"{vote.SuggestionId}"
            };

        public static Vote ToModel(this VoteDto vote) =>
            new Vote
            {
                Id = Guid.Parse(vote.Id),
                UserId = Guid.Parse(vote.UserId),
                SuggestionId = Guid.Parse(vote.SuggestionId)
            };
    }
}