

using MaleFashion.Application.Features.Categories.Command;
using MaleFashion.Application.Features.Products.Command;
using MaleFashion.Application.Features.Products.Query;
using MaleFashion.Application.Features.ProductVariants.Command;
using MaleFashion.Domain.Entites;
using MaleFashion.Web.Areas.Admin.Models;
using Mapster;

namespace MaleFashion.Web
{
    public class MapsterConfiguration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            // ==========================================
            // Category Mappings
            // ==========================================
            config.NewConfig<CategoryAddCommand, Category>();
            config.NewConfig<Category, CategoryModel>();

            config.NewConfig<CategoryModel, CategoryAddCommand>()
                .Ignore(dest => dest.ImageName);

            config.NewConfig<CategoryModel, CategoryUpdateCommand>()
                .Ignore(dest => dest.ImageName);

            // ==========================================
            // Product Create Mappings
            // ==========================================
            config.NewConfig<ProductCreateModel, ProductAddCommand>()
                .Ignore(dest => dest.ImageName)       // Set after file upload in controller
                .Ignore(dest => dest.GalleryImages)   // Set after file uploads in controller
                .Ignore(dest => dest.Variants);       // Deserialized from VariantsJson in controller

            // ==========================================
            // Product Update & Edit Mappings
            // ==========================================
            config.NewConfig<ProductEditModel, ProductUpdateCommand>()
                .Ignore(dest => dest.ImageName);      // Populated only if a new file is uploaded

            config.NewConfig<Product, ProductEditModel>()
                .Map(dest => dest.CurrentMainImage, src => src.ImageName)
                .Map(dest => dest.Images, src => src.Images)
                .Map(dest => dest.Variants, src => src.Variants);

            // ==========================================
            // Product Paging / Query Mapping
            // ==========================================
            config.NewConfig<ProductListModel, GetPagedProductsQuery>()
                .Map(dest => dest.SearchText, src => src.Search.Value)
                .Map(dest => dest.CategoryId, src => src.CategoryId)
                .Map(dest => dest.PageIndex, src => src.PageIndex)
                .Map(dest => dest.PageSize, src => src.PageSize);

            // ==========================================
            // Product Variant & Image Projections
            // ==========================================
            config.NewConfig<ProductVariant, ProductVariantItemModel>();
            config.NewConfig<ProductImage, ProductImageItemModel>();

            config.NewConfig<ProductVariantItemModel, ProductVariantAddCommand>();
            config.NewConfig<ProductVariantItemModel, ProductVariantUpdateCommand>();

            ////Order

            //config.NewConfig<OrderListModel, GetAllOrdersByPagingQuery>();

        }
    }
}
