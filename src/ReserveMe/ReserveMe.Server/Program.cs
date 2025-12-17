using System.Text;
using Application;
using Domain.Entities;
using Infrastructure;
using Infrastructure.DataSeeder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(
		builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
	.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddDefaultTokenProviders();

builder.Services
	.AddApplicationServices()
	.AddInfrastructureServices(builder.Configuration);

var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = jwtSection["Issuer"],
		ValidAudience = jwtSection["Audience"],
		IssuerSigningKey = new SymmetricSecurityKey(
			Encoding.UTF8.GetBytes(jwtSection["Key"]!))
	};
});

builder.Services.AddCors(options =>
{
	options.AddPolicy("CorsPolicy", builder =>
	builder.AllowAnyOrigin()
	.AllowAnyMethod()
	.AllowAnyHeader());
});

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReserveMe API", Version = "v1" });

	var securityScheme = new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Description = "Enter 'Bearer {token}'",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT",
		Reference = new OpenApiReference
		{
			Type = ReferenceType.SecurityScheme,
			Id = "Bearer"
		}
	};

	c.AddSecurityDefinition("Bearer", securityScheme);

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			securityScheme,
			Array.Empty<string>()
		}
	});
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(
		Path.Combine(app.Environment.ContentRootPath, "StaticFiles")),
	RequestPath = "/StaticFiles"
});

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	try
	{
		var appDbContext = services.GetRequiredService<ApplicationDbContext>();
		appDbContext.Database.Migrate();

		var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
		if (roleManager is null) throw new NullReferenceException(nameof(RoleManager<IdentityRole>));
		var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
		if (userManager is null) throw new NullReferenceException(nameof(UserManager<ApplicationUser>));

		var env = app.Services.GetRequiredService<IWebHostEnvironment>();

		var seedAppData = new AppDataSeeder(appDbContext, userManager, roleManager, env.ContentRootPath);
		await seedAppData.SeedAllAsync(CancellationToken.None);
	}
	catch (Exception ex)
	{
		var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
		logger.LogError(ex, "An error occurred during database initialisation.");

		throw;
	}
}

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
