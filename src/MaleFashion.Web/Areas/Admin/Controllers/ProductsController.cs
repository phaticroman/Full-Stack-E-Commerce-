using Cortex.Mediator;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Application.DTOs;
using MaleFashion.Application.Exceptions;
using MaleFashion.Application.Features.Categories.Query;
using MaleFashion.Application.Features.ProductImages.Command;
using MaleFashion.Application.Features.Products.Command;
using MaleFashion.Application.Features.Products.Query;
using MaleFashion.Application.Features.ProductVariants.Command;
using MaleFashion.Domain.Utilities;
using MaleFashion.Infrastructure.Extensions;
using MaleFashion.Web.Areas.Admin.Models;
using MaleFashion.Web.Codes;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Web;

namespace MaleFashion.Web.Areas.Admin.Controllers
{
    [Area("Admin"), Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IFileStorageService _fileStorageService;

        public ProductsController(
            ILogger<ProductsController> logger,
            IMediator mediator,
            IMapper mapper,
            IFileStorageService fileStorageService)
        {
            _logger = logger;
            _mediator = mediator;
            _mapper = mapper;
            _fileStorageService = fileStorageService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            ViewBag.Categories = await GetCategorySelectListAsync(cancellationToken);
            return View();
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var model = new ProductCreateModel
            {
                Categories = await GetCategorySelectListAsync(cancellationToken)
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategorySelectListAsync(cancellationToken);
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Please correct the errors and try again.", Type = ResponseTypes.Danger });
                return View(model);
            }

            try
            {
                var command = _mapper.Map<ProductAddCommand>(model);

                if (model.MainImageFile != null)
                {
                    using var stream = model.MainImageFile.OpenReadStream();
                    command.ImageName = await _fileStorageService.SaveImageAsync(
                        stream, model.MainImageFile.FileName, "products", cancellationToken);
                }

                if (model.GalleryFiles != null && model.GalleryFiles.Any())
                {
                    foreach (var file in model.GalleryFiles)
                    {
                        using var stream = file.OpenReadStream();
                        var fileName = await _fileStorageService.SaveImageAsync(
                            stream, file.FileName, "products", cancellationToken);
                        command.GalleryImages.Add(fileName);
                    }
                }

                if (!string.IsNullOrWhiteSpace(model.VariantsJson))
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var variants = JsonSerializer.Deserialize<List<ProductVariantDto>>(model.VariantsJson, options);
                    if (variants != null && variants.Any())
                    {
                        command.Variants = variants;
                    }
                }

                await _mediator.SendCommandAsync(command, cancellationToken);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Product created successfully.", Type = ResponseTypes.Success });

                return RedirectToAction(nameof(Index));
            }
            catch (DuplicateDataException dex)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = dex.Message, Type = ResponseTypes.Danger });
            }
            catch (DomainException domEx)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = domEx.Message, Type = ResponseTypes.Danger });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create product.");
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "An unexpected error occurred while creating the product.", Type = ResponseTypes.Danger });
            }

            model.Categories = await GetCategorySelectListAsync(cancellationToken);
            return View(model);
        }

        public async Task<IActionResult> Update(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _mediator.SendQueryAsync(new GetProductByIdWithDetailsQuery(id), cancellationToken);
                if (product == null)
                {
                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = "Product not found.", Type = ResponseTypes.Danger });
                    return RedirectToAction(nameof(Index));
                }

                var model = _mapper.Map<ProductEditModel>(product);
                model.Categories = await GetCategorySelectListAsync(cancellationToken);

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load product details for update.");
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Failed to load product details.", Type = ResponseTypes.Danger });
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(ProductEditModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategorySelectListAsync(cancellationToken);
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Please provide all required fields correctly.", Type = ResponseTypes.Danger });
                return View(model);
            }

            try
            {
                var command = _mapper.Map<ProductUpdateCommand>(model);
                var oldMainImage = model.CurrentMainImage;

                if (model.MainImageFile != null)
                {
                    using var stream = model.MainImageFile.OpenReadStream();
                    command.ImageName = await _fileStorageService.SaveImageAsync(
                        stream, model.MainImageFile.FileName, "products", cancellationToken);

                    if (!string.IsNullOrEmpty(oldMainImage))
                    {
                        await _fileStorageService.DeleteImageAsync(oldMainImage, "products", cancellationToken);
                    }
                }

                await _mediator.SendCommandAsync(command, cancellationToken);

                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Product updated successfully.", Type = ResponseTypes.Success });

                return RedirectToAction(nameof(Index));
            }
            catch (DuplicateDataException dex)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = dex.Message, Type = ResponseTypes.Danger });
            }
            catch (DomainException domEx)
            {
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = domEx.Message, Type = ResponseTypes.Danger });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update product.");
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Failed to update product.", Type = ResponseTypes.Danger });
            }

            model.Categories = await GetCategorySelectListAsync(cancellationToken);
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetPagedProducts([FromBody] ProductListModel model, CancellationToken cancellationToken)
        {
            try
            {
                var query = new GetPagedProductsQuery
                {
                    SearchText = model.Search.Value,
                    CategoryId = model.CategoryId,
                    SortText = model.FormatSortExpression("Name", "Name", "Category.Name", "Price", "DiscountPrice", "Brand", "IsActive"),
                    PageIndex = model.PageIndex,
                    PageSize = model.PageSize
                };

                var (items, total, totalDisplay) = await _mediator.SendQueryAsync(query, cancellationToken);

                var productData = items.Select(p => new string[]
                {
            p.ImageName ?? string.Empty,
            System.Web.HttpUtility.HtmlEncode(p.Name),
            System.Web.HttpUtility.HtmlEncode(p.Category?.Name ?? "Uncategorized"),
            p.Price.ToString("C"),
            p.DiscountPrice.HasValue ? p.DiscountPrice.Value.ToString("C") : "-",
            System.Web.HttpUtility.HtmlEncode(p.Brand),
            p.IsActive ? "Active" : "Inactive",
            p.Id.ToString()
                }).ToArray();

                return Json(new
                {
                    recordsTotal = total,
                    recordsFiltered = totalDisplay,
                    data = productData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch paged products.");
                return Json(DataTables.EmptyResult);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _mediator.SendQueryAsync(new GetProductByIdWithDetailsQuery(id), cancellationToken);
                if (product != null)
                {
                    if (!string.IsNullOrEmpty(product.ImageName))
                    {
                        await _fileStorageService.DeleteImageAsync(product.ImageName, "products", cancellationToken);
                    }

                    foreach (var img in product.Images)
                    {
                        await _fileStorageService.DeleteImageAsync(img.ImageName, "products", cancellationToken);
                    }

                    await _mediator.SendCommandAsync(new ProductDeleteCommand(id), cancellationToken);

                    TempData.Put(Constants.ResponseTempKey,
                        new ResponseModel { Message = "Product and associated assets deleted successfully.", Type = ResponseTypes.Success });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product.");
                TempData.Put(Constants.ResponseTempKey,
                    new ResponseModel { Message = "Failed to delete product.", Type = ResponseTypes.Danger });
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadGalleryImage(Guid productId, IFormFile imageFile, CancellationToken cancellationToken)
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                using var stream = imageFile.OpenReadStream();
                var savedFileName = await _fileStorageService.SaveImageAsync(stream, imageFile.FileName, "products", cancellationToken);

                var command = new ProductImageAddCommand
                {
                    ProductId = productId,
                    ImageName = savedFileName
                };

                var imageId = await _mediator.SendCommandAsync(command, cancellationToken);
                return Json(new { success = true, imageId, fileName = savedFileName });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to upload gallery image.");
                return StatusCode(500, "Error uploading gallery image.");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGalleryImage(Guid imageId, CancellationToken cancellationToken)
        {
            try
            {
                var removedFileName = await _mediator.SendCommandAsync(new ProductImageDeleteCommand(imageId), cancellationToken);

                if (!string.IsNullOrEmpty(removedFileName))
                {
                    await _fileStorageService.DeleteImageAsync(removedFileName, "products", cancellationToken);
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete gallery image.");
                return StatusCode(500, "Error deleting image.");
            }
        }

        private async Task<IEnumerable<SelectListItem>> GetCategorySelectListAsync(CancellationToken cancellationToken)
        {
            var categories = await _mediator.SendQueryAsync(new GetActiveCategoriesQuery(), cancellationToken);
            return categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            });
        }
        [HttpPost, ValidateAntiForgeryToken]
public async Task<IActionResult> AddVariant(AddProductVariantRequestModel model, CancellationToken cancellationToken)
{
    if (model == null || string.IsNullOrWhiteSpace(model.Size) || string.IsNullOrWhiteSpace(model.Color) || string.IsNullOrWhiteSpace(model.SKU))
    {
        return Json(new { success = false, message = "Please provide valid Size, Color, SKU, and Stock." });
    }

    try
    {
        var command = new ProductVariantAddCommand
        {
            ProductId = model.ProductId,
            Size = model.Size,
            Color = model.Color,
            SKU = model.SKU,
            StockQuantity = model.StockQuantity
        };

        Guid variantId = await _mediator.SendCommandAsync(command, cancellationToken);

        return Json(new
        {
            success = true,
            variant = new
            {
                id = variantId,
                size = model.Size,
                color = model.Color,
                sku = model.SKU,
                stockQuantity = model.StockQuantity
            }
        });
    }
    catch (DomainException dex)
    {
        return Json(new { success = false, message = dex.Message });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to add product variant.");
        return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
    }
}

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVariant(Guid id, CancellationToken cancellationToken)
        {
            try
            {
                await _mediator.SendCommandAsync(new ProductVariantDeleteCommand(id), cancellationToken);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product variant.");
                return Json(new { success = false, message = "Error deleting variant." });
            }
        }
    }
}
