using AutoMapper;
using Talabat.APIs.DTO;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.Order_Agggregate;

namespace Talabat.APIs.Helpers
{
	public class MappingProfiles : Profile
	{
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(D => D.ProductType, O => O.MapFrom(S => S.ProductType.Name))
                .ForMember(D => D.ProductBrand, O => O.MapFrom(S => S.ProductBrand.Name))
                .ForMember(D => D.PictureUrl, O => O.MapFrom<ProductPictureUrlResolver>());
            CreateMap<Talabat.Core.Entities.Identity.Address, AddressDto>().ReverseMap();
			CreateMap<AddressDto, Talabat.Core.Entities.Order_Agggregate.Address>().ReverseMap();

            CreateMap<CustomerBasketDto,CustomerBasket>().ReverseMap();
            CreateMap<BasketItemDto, BasketItem>().ReverseMap();

			CreateMap<Order, OrderToReturnDto>()
                .ForMember(O=>O.DeliveryMethod,O=>O.MapFrom(S=>S.DeliveryMethod.ShortName))
                .ForMember(O => O.DeliveryMethodCost, O => O.MapFrom(S => S.DeliveryMethod.Cost));
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(O=>O.ProductId,O=>O.MapFrom(S=>S.Product.ProductId))
                .ForMember(O => O.ProductName, O => O.MapFrom(S => S.Product.ProductName))
				.ForMember(O => O.PictureUrl, O => O.MapFrom<OrderItemPictureUrlResolver>());

		}
	}
}
