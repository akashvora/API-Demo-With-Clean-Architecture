﻿
namespace Movies.Api.Models.Responses
{
	public class MovieRatingResponse
	{
		public required Guid MovieId { get; init; }
		public required string Slug { get; init;}
		public required int rating { get; init; }
	}
}
