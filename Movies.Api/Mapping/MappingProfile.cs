using AutoMapper;
using Movies.Api.Models.Responses;
using Movies.Domain.Models;

namespace Movies.Api.Mapping
{
	public class MappingProfile:Profile
	{
		public MappingProfile() {
			CreateMap<MovieRating, MovieRatingResponse>();
			CreateMap<LoginUserToken, LoginResponse>();
		}
	}
}
