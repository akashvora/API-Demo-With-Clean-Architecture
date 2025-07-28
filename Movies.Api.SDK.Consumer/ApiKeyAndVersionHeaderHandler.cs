using System.Net.Http.Headers;

namespace Movies.Api.SDK.Consumer
{
	public class ApiKeyAndVersionHeaderHandler : DelegatingHandler
	{
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			// Add API Key
			request.Headers.Add("x-api-key", "d8566de3-b1a6-4a9b-b842-8e3887a82e41");

			// Add Media Type with Versioning
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")
			{
				Parameters = { new NameValueHeaderValue("api-version", "1.0") }
			});

			//  Optional: Alternative version header
			// request.Headers.Add("X-API-VERSION", "1.0");

			return await base.SendAsync(request, cancellationToken);
		}
	}

}
