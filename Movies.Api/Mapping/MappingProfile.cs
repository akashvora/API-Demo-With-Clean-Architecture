using AutoMapper;
using Movies.Api.Models.Requests;
using Movies.Api.Models.Responses;
using Movies.Application.Feature.Movies.Queries.GetAll;
using Movies.Domain.Models;

namespace Movies.Api.Mapping
{
	public class MappingProfile:Profile
	{
		public MappingProfile() {
			CreateMap<MovieRating, MovieRatingResponse>();
			CreateMap<LoginUserToken, LoginResponse>();

			//CreateMap<GetAllMoviesRequest, GetAllMoviesQuery>()
			//	.IncludeMembers(src => src.Paging);
		}
	}
}
