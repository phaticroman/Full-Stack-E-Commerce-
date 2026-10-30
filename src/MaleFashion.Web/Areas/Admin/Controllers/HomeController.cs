using Cortex.Mediator;
using MaleFashion.Application.Features.Dashboard.Query;
using MaleFashion.Infrastructure.Identity;
using MaleFashion.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaleFashion.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ApplicationUserManager _userManager;

        public HomeController(IMediator mediator, ApplicationUserManager userManager)
        {
            _mediator = mediator;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var stats = await _mediator.SendQueryAsync(new GetDashboardStatsQuery(), cancellationToken);
            return View(stats);
        }
    }
}
