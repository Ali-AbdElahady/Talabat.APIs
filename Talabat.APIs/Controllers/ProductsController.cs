using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTO;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Repositories;
using Talabat.Core.Specifications;

namespace Talabat.APIs.Controllers
{
	public class ProductsController : APIBaseController
	{
		private readonly IMapper mapper;
		private readonly IUnitOfWork unitOfWork;

		public ProductsController(IMapper mapper ,IUnitOfWork unitOfWork)
		{
			this.mapper = mapper;
			this.unitOfWork = unitOfWork;
		}
		//[Authorize(AuthenticationSchemes = "Bearer")] Or
		//[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
		[CachedAttribute(300)]
		[HttpGet]
		public async Task<ActionResult<Pagination<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams Params)
		 {
			var spec = new ProductWithBrandAndTypeSpecifications(Params);
			var Products = await unitOfWork.Reppository<Product>().GetAllWithSpecAsync(spec);
			var MappedProducts = mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(Products);
			//OkObjectResult result = new OkObjectResult(Products);
			//return result;
			var CountSpec = new ProductWithFilterationForCountAsync(Params);
			var Count = await unitOfWork.Reppository<Product>().GetCountWithSpecAsync(CountSpec);
			return Ok(new Pagination<ProductToReturnDto>(Params.PageSize,Params.pageIndex,MappedProducts,Count));
		}
		[HttpGet("{id}")]
		[ProducesResponseType(typeof(ProductToReturnDto), 200)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		public async Task<ActionResult<Product>> GetProductById(int id)
		{
			var spec = new ProductWithBrandAndTypeSpecifications(id);
			var Product = await unitOfWork.Reppository<Product>().GetEntityWithSpecAsync(spec);
			if (Product is null) return NotFound(new ApiResponse(404));
			var MappedProduct = mapper.Map<Product, ProductToReturnDto>(Product);
			return Ok(MappedProduct);
		}

		// GET ALL TYPES
		[HttpGet("Types")]
		public async Task<ActionResult<IReadOnlyList<ProductType>>> GetTypes()
		{
			var Types = await unitOfWork.Reppository<ProductType>().GetAllAsync();
			return Ok(Types);
		}

		// GET ALL BRANDS
		[HttpGet("Brands")]
		public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
		{
			var Brands = await unitOfWork.Reppository<ProductBrand>().GetAllAsync();
			return Ok(Brands);
		}
	}
}
