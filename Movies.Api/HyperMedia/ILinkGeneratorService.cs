using Movies.Shared.Models.HyperMedia;

namespace Movies.Api.HyperMedia
{
	public interface ILinkGeneratorService
	{
		List<Link> GenerateLinks(object resource, string controllerName, Dictionary<string, (string action, string method, string[] paramNames)> routes);

	}
}
