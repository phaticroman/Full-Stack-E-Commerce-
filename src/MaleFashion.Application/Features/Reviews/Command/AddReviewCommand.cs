using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Reviews.Command
{
    public class AddReviewCommand : ICommand
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Guid CustomerId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }

        public AddReviewCommand() { }

        public AddReviewCommand(Guid productId, Guid customerId, int rating, string? comment)
        {
            ProductId = productId;
            CustomerId = customerId;
            Rating = rating;
            Comment = comment;
        }
    }
}
