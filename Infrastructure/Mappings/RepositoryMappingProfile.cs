using AutoMapper;
using Movies.Application.Feature.Movies.Queries.GetAll;
using Movies.Shared.Models.Paging;

namespace Infrastructure.Mappings
{
	public class RepositoryMappingProfile : Profile
	{
		public RepositoryMappingProfile()
		{
			CreateMap<GetAllMoviesQuery, GetAllMoviesOptions>()
			   .ForMember(dest => dest.SortField, opt => opt.Ignore())
			   .ForMember(dest => dest.SortOrder, opt => opt.Ignore())
			   .ForMember(dest => dest.Paging, opt => opt.MapFrom((src, _) => new PageRequest(src.Page, src.PageSize)))
			.ReverseMap()
			   .ForMember(dest => dest.SortBy, opt => opt.Ignore())
			   .ForMember(dest => dest.Page, opt => opt.MapFrom(src => src.Paging.Page))
			   .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.Paging.PageSize))
			   .ForSourceMember(src => src.SortField, opt => opt.DoNotValidate())
			   .ForSourceMember(src => src.SortOrder, opt => opt.DoNotValidate());


			//CreateMap<PageRequest, GetAllMoviesQuery>()
			//	.ForMember(dest => dest.Page, opt => opt.MapFrom(src => src.Page))
			//	.ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize));

		}
	}
}
