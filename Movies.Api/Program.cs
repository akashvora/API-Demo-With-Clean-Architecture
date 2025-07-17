using AutoMapper;
using Infrastructure.Authentication;
using Infrastructure.DependecyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Movies.Api.Common.AuthenticationEnums;
using Movies.Api.Mapping;
using Movies.Api.Middleware;
using Movies.Application.Database;
using Movies.Application.DependencyInjection;
using Movies.Application.Feature.Authentication.Interfaces;
using Movies.Infrastructure.Database;
using NpgsqlTypes;
using Serilog;
using Serilog.Sinks.PostgreSQL;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Build dynamic PostgreSQL connection string
var config = builder.Configuration;
var connectionString1 = $"Host={config["LoggingDb:Host"]};Port={config["LoggingDb:Port"]};Database={config["LoggingDb:Database"]};Username={config["LoggingDb:Username"]};Password={config["LoggingDb:Password"]}";
// Define PostgreSQL log columns
var columnWriters = new Dictionary<string, ColumnWriterBase>
{
	{ "message", new RenderedMessageColumnWriter(NpgsqlDbType.Text) },
	{ "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
	{ "raise_date", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
	{ "exception", new ExceptionColumnWriter(NpgsqlDbType.Text) },
	{ "properties", new LogEventSerializedColumnWriter(NpgsqlDbType.Jsonb) },
	{ "machine_name", new SinglePropertyColumnWriter("MachineName", PropertyWriteMethod.ToString, NpgsqlDbType.Text) }
};
// Configure Serilog globally
Log.Logger = new LoggerConfiguration()
	.Enrich.FromLogContext()
	.WriteTo.Console()
	.WriteTo.PostgreSQL(
		connectionString: connectionString1,
		tableName: "Logs",
		columnOptions: columnWriters,
		needAutoCreateTable: true)
	.WriteTo.Map(
		keySelector: logEvent => logEvent.Timestamp.Year.ToString(),
		configure: (year, wt) => wt.File(
			path: $"Logs/{year}/log-{DateTime.UtcNow:yyyy-MM-dd}.txt",
			rollingInterval: RollingInterval.Day,
			//retainedFileCountLimit: 30,
			//fileSizeLimitBytes: 10_000_000,
			//rollOnFileSizeLimit: true,
			fileSizeLimitBytes: null,           // 🔓 Unlimited file size
			retainedFileCountLimit: null,       // 🔓 Keep all log files
			rollOnFileSizeLimit: false,         // Not needed when size is unlimited
			shared: true,
			flushToDiskInterval: TimeSpan.FromSeconds(1)),
		sinkMapCountLimit: 10)

	.CreateLogger();

builder.Host.UseSerilog(); // Register Serilog globally


builder.Services.AddAutoMapper(cfg =>
{
	cfg.AddProfile<MappingProfile>();
});
//builder.Services.AddAutoMapper(typeof(Program)); // use this if all mapping profiles in same assembly


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddAuthentication(options => {
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options => { 
	var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = jwtSettings.Issuer,
		ValidAudience = jwtSettings.Audience,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
		NameClaimType = "userid"
	};
});

builder.Services.AddAuthorization(options =>
{
	options.AddPolicy(Policies.AdminOnly, policy =>
		policy.RequireRole(Roles.Admin));

	options.AddPolicy(Policies.TrustMemberOnly, policy =>
		policy.RequireRole(Roles.TrustMember));

	options.AddPolicy(Policies.AdminOrTrustMember, policy =>
	policy.RequireRole(Roles.Admin, Roles.TrustMember));

});


// Register application services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastutureServices();


var connectionString = builder.Configuration["Database:ConnectionString"];
if (string.IsNullOrEmpty(connectionString))
{
	throw new InvalidOperationException("Database connection string is missing!");
}
builder.Services.AddDatabase(connectionString);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var dbInitializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
	await dbInitializer.InitializeAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	// below two auto mapper line to check if mapped model and its propery has error at startup . maybe throws error in start up. so only use for development
	var mapperConfig = app.Services.GetRequiredService<AutoMapper.IConfigurationProvider>();
	mapperConfig.AssertConfigurationIsValid();

	app.UseSwagger();
	app.UseSwaggerUI();
}

// Register your custom exception middleware
app.UseExceptionHandlingMiddleware();


app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ValidationMappingMiddleware>();
app.MapControllers();

app.Run();
