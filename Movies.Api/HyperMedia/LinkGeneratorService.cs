using Movies.Shared.Models.HyperMedia;
using System.Reflection;

namespace Movies.Api.HyperMedia;

public class LinkGeneratorService : ILinkGeneratorService
{
	private readonly LinkGenerator _generator;
	private readonly IHttpContextAccessor _accessor;

	public LinkGeneratorService(LinkGenerator generator, IHttpContextAccessor accessor)
	{
		_generator = generator;
		_accessor = accessor;
	}

	public List<Link> GenerateLinks(object resource, string controllerName, Dictionary<string, (string action, string method, string[] paramNames)> routes)
	{
		var httpContext = _accessor.HttpContext!;
		var resourceType = resource.GetType();

		var links = new List<Link>();

		foreach (var kvp in routes)
		{
			var rel = kvp.Key;
			var (action, method, paramNames) = kvp.Value;

			// Dynamically build routeValues object
			var routeValues = new Dictionary<string, object?>();

			foreach (var paramName in paramNames)
			{
				var prop = resourceType.GetProperty(paramName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
				var value = prop?.GetValue(resource);
				if (value is not null)
					routeValues[paramName] = value;
			}

			var uri = _generator.GetUriByAction(httpContext, action, controllerName, routeValues);

			if (uri != null)
			{
				links.Add(new Link(uri, rel, method));
			}
		}

		return links;
	}
}