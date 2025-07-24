using Movies.Api.HyperMedia;

namespace Movies.Api.ApiDepedencyInjection
{
	public static class ApiServicesExtension
	{
		public static IServiceCollection AddApiServicesExtension(this IServiceCollection Services) {
			Services.AddHttpContextAccessor();
			Services.AddScoped<ILinkGeneratorService, LinkGeneratorService>();
			return Services;
		}
	}
}
