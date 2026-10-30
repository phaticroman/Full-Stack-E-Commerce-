using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Reviews.Command
{
    public class UpdateReviewCommand : ICommand
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }

        public UpdateReviewCommand() { }

        public UpdateReviewCommand(Guid id, Guid customerId, int rating, string? comment)
        {
            Id = id;
            CustomerId = customerId;
            Rating = rating;
            Comment = comment;
        }
    }
}
