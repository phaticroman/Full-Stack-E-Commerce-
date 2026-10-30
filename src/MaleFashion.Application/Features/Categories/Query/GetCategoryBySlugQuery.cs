using Cortex.Mediator.Queries;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Categories.Query
{
    public class GetCategoryBySlugQuery : IQuery<Category?>
    {
        public string Slug { get; }

        public GetCategoryBySlugQuery(string slug)
        {
            Slug = slug;
        }
    }
}
