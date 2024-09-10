using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core;
using Talabat.Core.Repositories;
using Talabat.Core.Services;
using Talabat.Repository;
using Talabat.Service;

namespace Talabat.APIs.Extensions
{
	public static class AplicationServicesExtension
	{
        public static IServiceCollection AddAplicationServices(this IServiceCollection Services)
		{
			// Create one Object per Run time
			Services.AddSingleton(typeof(IResponseCacheService),typeof(ResponseCacheService));

			Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
			Services.AddScoped(typeof(IOrderService), typeof(OrderService));
			Services.AddScoped(typeof(IPaymentService), typeof(PaymentService));
			Services.AddSwaggerGen(c =>
			{
				c.CustomSchemaIds(type => type.FullName);
			});

			//Services.AddScoped(typeof(IGenericReppository<>), typeof(GenericRepository<>));
			//builder.Services.AddScoped<IGenaricReppository<Product>, GenaricRepository<Product>>();
			//builder.Services.AddScoped<IGenaricReppository<ProductBrand>, GenaricRepository<ProductBrand>>();
			Services.AddScoped(typeof(IBasketRepository),typeof(BasketRepository));

			//builder.Services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));
			Services.AddAutoMapper(typeof(MappingProfiles));
			Services.Configure<ApiBehaviorOptions>(options =>
			{
				options.InvalidModelStateResponseFactory = (actionContext) =>
				{
					// ModelState => Dic [keyValuePair]
					// Key => Name Of Param
					// Value => Errors
					var errors = actionContext.ModelState.Where(p => p.Value.Errors.Count() > 0)
														.SelectMany(p => p.Value.Errors)
														.Select(e => e.ErrorMessage);
					var validationErrorRespons = new ApiValidationErrorResponce()
					{
						Errors = errors
					};
					return new BadRequestObjectResult(validationErrorRespons);
				};
			});
			return Services;
		}
    }
}
