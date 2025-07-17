using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Application.Common
{
	public class Result<T>
	{
		public bool IsSuccess { get; set; }
		public T? Value { get; init; }
		public ProblemDetails? Problem { get;  init; }
		public bool IsFailure => !IsSuccess;

		private Result(bool isSuccess, T? value, ProblemDetails? problem)
		{
			IsSuccess = isSuccess;
			Value  = value;
			Problem = problem;
		}

		public static Result<T> Success(T value) => new(true, value, null);
		public static Result<T> Failure(string title, string detail, int statusCode, string? instance = null)
		{
			var problem = new ProblemDetails
			{
				Title = title,
				Detail = detail,
				Status = statusCode,
				Type = $"https://httpstatuses.com/{statusCode}",
				Instance = instance
			};
			return new Result<T>(false, default, problem);
		}
		public static Result<T> Failure(ProblemDetails problem) => new(false,default,problem);	
	}
}
