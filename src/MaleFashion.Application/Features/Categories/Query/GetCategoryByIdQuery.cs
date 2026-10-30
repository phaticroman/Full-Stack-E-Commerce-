using Cortex.Mediator.Queries;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Categories.Query
{
    public class GetCategoryByIdQuery : IQuery<Category?>
    {
        public Guid Id { get; }

        public GetCategoryByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
