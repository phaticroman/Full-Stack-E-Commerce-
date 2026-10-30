using Cortex.Mediator.DependencyInjection;
using MaleFashion.Application.Features.Categories.Command;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Infrastructure.Extensions
{
    public static class MediatorConfigurations
    {
        public static void AddCortexMediatorConfiguration(this IServiceCollection services)
        {
            services.AddCortexMediator(
                new[] { typeof(CategoryAddCommand) },
                options => options.AddDefaultBehaviors()
            );
        }
    }
}
