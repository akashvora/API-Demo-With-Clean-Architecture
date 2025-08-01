﻿using Movies.Domain.Models;
using Movies.Api.Models.Responses;
using Movies.Api.Models.Requests;

namespace Movies.Api.Mapping
{
	public static class ContractMapping
	{
		public static Movie MapToMovie(this CreateMovieRequest request)
		{
			return new Movie
			{
				Id = Guid.NewGuid(),
				Title = request.Title,
				YearOfRelease = request.YearOfRelease,
				Genres = request.Genres.ToList(),
				
			};
		}
		public static MovieResponse MapToResponse(this Movie movie)
		{
				return new MovieResponse
			{
				Id = movie.Id,
				Title = movie.Title,
				Slug = movie.Slug,
				YearOfRelease = movie.YearOfRelease,
				Genres = movie.Genres,
				Rating = movie.Rating,
				UserRating = movie.UserRating,
			};
		}

		public static MoviesResponse MapToResponse(this IEnumerable<Movie> movies)
		{
			return new MoviesResponse
			{
				Items = movies.Select(MapToResponse)
			};
		}

		public static Movie MapToMovie(this UpdateMovieRequest request, Guid id)
		{
			return new Movie
			{
				Id = id,
				Title = request.Title,
				YearOfRelease = request.YearOfRelease,
				Genres = request.Genres.ToList(),
			};
		}
	}
}
