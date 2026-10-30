using Cortex.Mediator.Commands;
using MaleFashion.Domain.Entites;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Application.Features.Categories.Command
{
    public class CategoryAddCommand : ICommand<Guid>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? ImageName { get; set; }
        public bool IsActive { get; set; } = true;
        public int DisplayOrder { get; set; }
    }
}
