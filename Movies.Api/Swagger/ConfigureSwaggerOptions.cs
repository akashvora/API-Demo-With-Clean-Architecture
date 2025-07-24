using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Movies.Api.Swagger
{
	public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
	{
		private readonly IApiVersionDescriptionProvider _provider;

		public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
		{
			_provider = provider;
		}

		public void Configure(SwaggerGenOptions options)
		{
			foreach (var desc in _provider.ApiVersionDescriptions)
			{
				options.SwaggerDoc(desc.GroupName, new OpenApiInfo
				{
					Title = $"Movies API {desc.ApiVersion}",
					Version = desc.ApiVersion.ToString(),
					Description = desc.IsDeprecated ? "This version is deprecated." : "Stable version"
				});
			}

			options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
			{
				In=ParameterLocation.Header,
				Description="Please provide a valid token",
				Name="Authorization",
				Type=SecuritySchemeType.Http,
				BearerFormat="JWT",
				Scheme="Bearer"
			});

			options.AddSecurityRequirement(new OpenApiSecurityRequirement
			{
				{
					new OpenApiSecurityScheme
					{
						Reference = new OpenApiReference
						{
							Type=ReferenceType.SecurityScheme,
							Id="Bearer"
						}
					},
					Array.Empty<string>()
				}
			});
		}
	}
}

