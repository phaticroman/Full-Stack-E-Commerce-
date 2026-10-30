using Cortex.Mediator;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Application.Exceptions;
using MaleFashion.Application.Features.Categories.Command;
using MaleFashion.Application.Features.Categories.Query;
using MaleFashion.Domain.Utilities;
using MaleFashion.Infrastructure.Extensions;
using MaleFashion.Web.Areas.Admin.Models;
using MaleFashion.Web.Codes;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Web;

namespace MaleFashion.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ILogger<CategoriesController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public CategoriesController(
            ILogger<CategoriesController> logger,
            IMediator mediator,
            IMapper mapper,
            IFileStorageService fileStorageService)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View(new CategoryModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Please provide all required information.", Type = ResponseTypes.Danger });
                return View(model);
            }

            try
            {
                var command = _mapper.Map<CategoryAddCommand>(model);

                if (model.ImageFile != null)
                {
                    using var stream = model.ImageFile.OpenReadStream();
                    command.ImageName = await _fileStorageService.SaveImageAsync(
                        stream, model.ImageFile.FileName, "categories", cancellationToken);
                }

                await _mediator.SendCommandAsync(command, cancellationToken);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Category successfully created.", Type = ResponseTypes.Success });

                return RedirectToAction(nameof(Index));
            }
            catch (DuplicateDataException oex)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = oex.Message, Type = ResponseTypes.Danger });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create category.");
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Failed to create category.", Type = ResponseTypes.Danger });
            }

            return View(model);
        }

        public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _mediator.SendQueryAsync(new GetCategoryByIdQuery(id), cancellationToken);

                if (category == null)
                {
                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = "Category not found.", Type = ResponseTypes.Danger });
                    return RedirectToAction(nameof(Index));
                }

                var model = _mapper.Map<CategoryModel>(category);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load category.");
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Failed to load category.", Type = ResponseTypes.Danger });

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(CategoryModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Please provide all required information.", Type = ResponseTypes.Danger });
                return View(model);
            }

            try
            {
                var command = _mapper.Map<CategoryUpdateCommand>(model);
                var oldImageName = model.ImageName;

                if (model.ImageFile != null)
                {
                    using var stream = model.ImageFile.OpenReadStream();
                    command.ImageName = await _fileStorageService.SaveImageAsync(
                        stream, model.ImageFile.FileName, "categories", cancellationToken);

                    if (!string.IsNullOrEmpty(oldImageName))
                    {
                        await _fileStorageService.DeleteImageAsync(oldImageName, "categories", cancellationToken);
                    }
                }

                await _mediator.SendCommandAsync(command, cancellationToken);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Category successfully updated.", Type = ResponseTypes.Success });

                return RedirectToAction(nameof(Index));
            }
            catch (DuplicateDataException oex)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = oex.Message, Type = ResponseTypes.Danger });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update category.");
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Failed to update category.", Type = ResponseTypes.Danger });
            }

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetPagedCategories([FromBody] CategoryListModel model, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetPagedCategoriesQuery
                {
                    SearchText = model.Search.Value,
                    SortText = model.FormatSortExpression("Name", "Name", "Slug", "DisplayOrder"),
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize
                };

                var (items, total, totalDisplay) = await _mediator.SendQueryAsync(query, cancellationToken);

                var categories = new
                {
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = items.Select(item => new string[]
                    {
                        HttpUtility.HtmlEncode(item.Name),
                        HttpUtility.HtmlEncode(item.Slug),
                        item.IsActive ? "Active" : "Inactive",
                        item.DisplayOrder.ToString(),
                        item.Id.ToString()
                    }).ToArray()
                };

                return Json(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get category list.");
                return Json(DataTables.EmptyResult);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.SendCommandAsync(new CategoryDeleteCommand { Id = id }, cancellationToken);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Category successfully deleted.", Type = ResponseTypes.Success });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete category.");
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Failed to delete category.", Type = ResponseTypes.Danger });
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
