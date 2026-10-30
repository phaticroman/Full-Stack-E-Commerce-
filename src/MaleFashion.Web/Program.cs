using Cortex.Mediator.DependencyInjection;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Utilities;
using MaleFashion.Infrastructure.Data;
using MaleFashion.Infrastructure.Extensions;
using MaleFashion.Infrastructure.Services;
using Mapster;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection;
using MaleFashion.Application.Contracts.Services;


Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/web-log-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    var migrationsAssembly = Assembly.GetAssembly(typeof(ApplicationDbContext));

    builder.Services.AddInfrastructureDependency(builder.Environment.WebRootPath);

    builder.Services.AddDbContext(connectionString, migrationsAssembly);


    builder.Services.AddDatabaseDeveloperPageExceptionFilter();


    #region Serilog Configuration

    builder.Host.UseSerilog((context, lc) => lc
        .MinimumLevel.Debug()
        .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(context.Configuration)
    );
    #endregion


    #region Cortex.Mediator Configuration
    builder.Services.AddCortexMediatorConfiguration();
    #endregion

    #region Mapster Configuration

    // Custom Configuration
    //var config = TypeAdapterConfig.GlobalSettings;
    //config.Scan(typeof(MapsterConfiguration).Assembly);
    //builder.Services.AddSingleton(config);
    //builder.Services.AddScoped<IMapper, ServiceMapper>();

    // Default Configuration
    builder.Services.AddMapster();

    #endregion

    #region Docker IP Correction
    builder.WebHost.UseUrls("http://*:80");
    #endregion

    #region Identity Configuration
    builder.Services.AddIdentity();
    builder.Services.AddAuthorization();
    //builder.Services.AddPolicyAuthorization();
    #endregion

    #region AppSettings Configuration
    builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
    #endregion

    #region Recaptcha Configuration
    builder.Services.Configure<RecaptchaSettings>(builder.Configuration.GetSection("RecaptchaSettings"));
    builder.Services.AddHttpClient<IRecaptchaService, RecaptchaService>();
    #endregion

    builder.Services.AddControllersWithViews();


    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseMigrationsEndPoint();
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapStaticAssets();
    app.UseStaticFiles();

    app.MapControllerRoute(
        name: "root",
        pattern: "",
        defaults: new { area = "Customer", controller = "Home", action = "Index" })
        .WithStaticAssets();

    app.MapControllerRoute(
            name: "area",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    //app.MapRazorPages()
    //   .WithStaticAssets();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {

            await MaleFashion.Infrastructure.Data.Seeds.DatabaseSeeder.SeedRolesAndAdminAsync(services);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "Error while seeding admin user and roles.");
        }
    }
    Log.Information("Application Starting Up");
    app.Run();
}

catch (Exception e)
{
    Log.Fatal(e, "Application Curshed!");
}
finally
{
    Log.CloseAndFlush();
}