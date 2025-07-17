namespace Movies.Api.Models.Responses
{
	public class ValidationFailureResponse
	{
		public required IEnumerable<ValidationResponse> Errors { get; init; } = Enumerable.Empty<ValidationResponse>();
		
		//public required string Message { get; init; } = string.Empty;
		//public required int StatusCode { get; init; }
		//public required string ErrorCode { get; init; } = string.Empty;
		//public required string ErrorType { get; init; } = string.Empty;
		//public required DateTime Timestamp { get; init; } = DateTime.UtcNow;
		//public required string RequestId { get; init; } = string.Empty;
		//public required string TraceId { get; init; } = string.Empty;
	}
	public class ValidationResponse
	{
		public required string PropertyName { get; init; } = string.Empty;
		public required string Message { get; init; } = string.Empty;
	}
}
