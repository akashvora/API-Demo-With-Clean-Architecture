using AutoMapper;
using Movies.Application.Feature.Movies.Queries.GetAll;
using Movies.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Mappings
{
	public class RepositoryMappingProfile : Profile
	{
		public RepositoryMappingProfile()
		{
			CreateMap<GetAllMoviesOptions, GetAllMoviesQuery>().ReverseMap();
		}
	}
}
