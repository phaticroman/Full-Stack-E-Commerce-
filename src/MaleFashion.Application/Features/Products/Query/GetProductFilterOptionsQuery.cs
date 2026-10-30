using Cortex.Mediator.Queries;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Products.Query
{
    public class GetProductFilterOptionsQuery : IQuery<(IList<string> Sizes, IList<string> Colors)>
    {
    }
}
