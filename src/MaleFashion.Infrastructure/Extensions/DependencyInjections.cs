using MaleFashion.Application.Contracts;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Domain.Utilities;
using MaleFashion.Infrastructure.Data;
using MaleFashion.Infrastructure.Data.Respositories;
using MaleFashion.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Infrastructure.Extensions
{
    public static class DependencyInjections
    {
        public static IServiceCollection AddInfrastructureDependency(this IServiceCollection services, string rootPath)
        {
            services.AddScoped<IApplicationUnitOfwork, ApplicationUnitOfWork>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICartRepository, CartRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddSingleton<IFileStorageService, FileStorageService>(x => new FileStorageService(rootPath));

            

            return services;
        }
    }
}
