﻿namespace Movies.Api.Models.Requests
{
	public class GetAllMoviesRequest
	{
		public required string? Title { get; init; }
		public required int? Year { get; init; }
	}
}
