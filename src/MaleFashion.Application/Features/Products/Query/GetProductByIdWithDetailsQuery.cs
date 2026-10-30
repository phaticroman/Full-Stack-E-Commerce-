using Cortex.Mediator.Queries;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Query
{
    public class GetProductByIdWithDetailsQuery : IQuery<Product?>
    {
        public Guid Id { get; }

        public GetProductByIdWithDetailsQuery(Guid id)
        {
            Id = id;
        }
    }
}
