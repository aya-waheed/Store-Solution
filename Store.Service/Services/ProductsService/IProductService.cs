using Store.Repository.Specifications.Products;
using Store.Service.Helper;
using Store.Service.Services.ProductsService.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.ProductsService
{
    public interface IProductService
    {

        Task<ProductDetailsDto> GetProductByIdAsync(int? productId);

        Task<PaginatedResultDto<ProductDetailsDto>> GetAllProductsAsync(ProductSpecification specs);

        Task<IReadOnlyList<BrandTypeDetailsDto>> GetAllBrandsAsync();

        Task<IReadOnlyList<BrandTypeDetailsDto>> GetAllTypesAsync();


    }
}
