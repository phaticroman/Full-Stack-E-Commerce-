using Cortex.Mediator.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Reviews.Command
{
    public class DeleteReviewCommand : ICommand
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }

        public DeleteReviewCommand() { }

        public DeleteReviewCommand(Guid id, Guid customerId)
        {
            Id = id;
            CustomerId = customerId;
        }
    }
}
