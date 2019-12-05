using System;

namespace Votes.Models
{
    public class Vote
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SuggestionId { get; set; }
    }
}