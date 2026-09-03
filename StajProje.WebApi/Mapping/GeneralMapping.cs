using AutoMapper;
using StajProje.WebApi.Dtos.AboutDtos;
using StajProje.WebApi.Dtos.CategoryDtos;
using StajProje.WebApi.Dtos.FeatureDtos;
using StajProje.WebApi.Dtos.MessageDtos;
using StajProje.WebApi.Dtos.ProductDtos;
using StajProje.WebApi.Dtos.ReservationDtos;
using StajProje.WebApi.Dtos.ImageDtos;
using StajProje.WebApi.Dtos.ContactDtos;
using StajProje.WebApi.Entities;

namespace StajProje.WebApi.Mapping
{
    public class GeneralMapping : Profile
    {
        public GeneralMapping()
        {
            CreateMap<Reservation, CreateReservationDto>().ReverseMap();
            CreateMap<Reservation, UpdateReservationDto>().ReverseMap();
            CreateMap<Reservation, GetByIdReservationDto>().ReverseMap();
            CreateMap<Reservation, ResultReservationDto>().ReverseMap();

            CreateMap<Category, CreateCategoryDto>().ReverseMap();
            CreateMap<Category, UpdateCategoryDto>().ReverseMap();

            CreateMap<Feature, ResultFeatureDto>().ReverseMap();
            CreateMap<Feature, UpdateFeatureDto>().ReverseMap();
            CreateMap<Feature, GetByIdFeatureDto>().ReverseMap();
            CreateMap<Feature, CreateFeatureDto>().ReverseMap();

            CreateMap<Message, ResultMessageDto>().ReverseMap();
            CreateMap<Message, UpdateMessageDto>().ReverseMap();
            CreateMap<Message, GetByIdMessageDto>().ReverseMap();
            CreateMap<Message, CreateMessageDto>().ReverseMap();

            CreateMap<Product, ResultProductDto>().ReverseMap();
            CreateMap<Product, UpdateProductDto>().ReverseMap();
            CreateMap<Product, GetByIdProductDto>().ReverseMap();
            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<Product, ResultProductWithCategoryDto>().ForMember(x=>x.CategoryName, y=>y.MapFrom(z=>z.Category.CategoryName)).ReverseMap();

            CreateMap<About, CreateAboutDto>().ReverseMap();
            CreateMap<About, UpdateAboutDto>().ReverseMap();
            CreateMap<About, GetByIdAboutDto>().ReverseMap();
            CreateMap<About, ResultAboutDto>().ReverseMap();

            CreateMap<Image, CreateImageDto>().ReverseMap();
            CreateMap<Image, UpdateImageDto>().ReverseMap();
            CreateMap<Image, GetByIdImageDto>().ReverseMap();
            CreateMap<Image, ResultImageDto>().ReverseMap();

            CreateMap<Contact, CreateContactDto>().ReverseMap();
            CreateMap<Contact, UpdateContactDto>().ReverseMap();
            CreateMap<Contact, GetByIdContactDto>().ReverseMap();
            CreateMap<Contact, ResultContactDto>().ReverseMap();
        }
    }
}
