
using Microsoft.Extensions.DependencyInjection;
using Movies.Api.Models.Requests;
using Movies.Api.SDK;
using Movies.Api.SDK.Consumer;
using Newtonsoft.Json;
using Refit;
using System;
using System.Net.Http.Headers;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;


//var moviesApi = RestService.For<IMoviesApi>("https://localhost:7227");


// Add API Key and Versioning headers ... here we used refit http client factory packahe its json package
var services = new ServiceCollection();

services
	.AddHttpClient()
	.AddSingleton<AuthTokenProvider>();
// ------------------------  Refit HttpClient Factory with custom headers With service provider -----------------------------
services.AddTransient<ApiKeyAndVersionHeaderHandler>();

services.AddRefitClient<IMoviesApi>(x => new RefitSettings
	{
		ContentSerializer = new NewtonsoftJsonContentSerializer(new JsonSerializerSettings
		{
			ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
			NullValueHandling = NullValueHandling.Ignore
		}),
	AuthorizationHeaderValueGetter = async (request, cancellationToken) =>
	{
		var tokenProvider = x.GetRequiredService<AuthTokenProvider>();
		return await tokenProvider.GetTokenAsync(string.Empty, default, new string [] {});
	
	}

	//AuthorizationHeaderValueGetter = (request, canncellationToke) => Task.FromResult
	//AuthorizationHeaderValueGetter = (request, cancellationToken) => Task.FromResult("Bearer your-static-token")

})
	.ConfigureHttpClient(c => c.BaseAddress = new Uri("https://localhost:7227"))
	//.ConfigureHttpClient(c => c.BaseAddress = client.BaseAddress)
	.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
	{
		ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true // Ignore SSL errors for development
	}).AddHttpMessageHandler<ApiKeyAndVersionHeaderHandler>();


//--------------------   Http client with refit easy way              -------------------------------------//

// Build HttpClient manually
var client = new HttpClient
{
	BaseAddress = new Uri("https://localhost:7227") // HTTPS!
};


//  Add API Key
client.DefaultRequestHeaders.Add("x-api-key", "d8566de3-b1a6-4a9b-b842-8e3887a82e41");

//  Add Media Type with Versioning
client.DefaultRequestHeaders.Accept.Add(
	new MediaTypeWithQualityHeaderValue("application/json")
	{
		Parameters = { new NameValueHeaderValue("api-version", "1.0") }
	}
);

//// OR add Header-based Version
//client.DefaultRequestHeaders.Add("X-API-VERSION", "1.0");



// Build Refit client
var moviesApi = RestService.For<IMoviesApi>(client);

var provider = services.BuildServiceProvider();
var moviesApiWithProvider = provider.GetRequiredService<IMoviesApi>();

//  Call the endpoint
//var movie = await moviesApi.GetMovieAsync("harry-potter-2021");
var movie = await moviesApiWithProvider.GetMovieAsync("harry-potter-2021");

//Console.WriteLine($"Movie Title: {movie.Title}");
//Console.WriteLine(JsonSerializer.Serialize(movie));

var newMovieRequest = await moviesApiWithProvider.CreateMovieAsync( new CreateMovieRequest { 
	Title="Pink Panther",
	YearOfRelease = 2005,
	Genres = new[] { "Fantasy", "Adventure", "Mystery" }
});

await moviesApiWithProvider.UpdateMovieAsync(newMovieRequest.Id,new UpdateMovieRequest
{
	Title = "Pink Panther",
	YearOfRelease = 2005,
	Genres = new[] { "Fantasy", "Adventure", "tauba tauba" }
});

await moviesApiWithProvider.DeleteMovieAsync(newMovieRequest.Id);

var request =new GetAllMoviesRequest
{
 	Page = 1,
	PageSize = 10,
	SortBy = null,
	Title = null,
	Year = null

};

// var movies = await moviesApi.GetMoviesAsync(request);
//var movies = await moviesApiWithProvider.GetMoviesAsync(request);

//foreach ( var response in movies.Items)
//{
//	//Console.WriteLine($"Movie Title: {response.Title}"); 
//	Console.WriteLine(JsonSerializer.Serialize(response));
//}

