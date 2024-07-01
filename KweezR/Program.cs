using System.Text;
using Contracts;
using Entities.Models;
using KweezR.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Services;
using Services.Contract;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<RepositoryContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection")));
builder.Services.AddControllers()
    .AddApplicationPart(typeof(KweezR.Presentation.AssemblyReference).Assembly);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSignalR();
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
	options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddCors(opt => opt.AddDefaultPolicy(builder =>
{
    builder.AllowAnyHeader();
    builder.AllowAnyMethod();
	builder.WithOrigins("https://localhost:7125", "http://localhost:5033");
    builder.AllowCredentials();
    builder.WithExposedHeaders("X-Pagination");
}));

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
	options.User.RequireUniqueEmail = true;
	options.Password.RequireUppercase = true;
	options.Password.RequireNonAlphanumeric = true;
})
 .AddDefaultTokenProviders()
.AddEntityFrameworkStores<RepositoryContext>();
builder.Services.AddAuthentication();
builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
	options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateIssuerSigningKey = true,
		ValidateLifetime = true,

		ValidIssuer = builder.Configuration.GetSection("JwtSettings")["ValidIssuer"],
		ValidAudience = builder.Configuration.GetSection("JwtSettings")["ValidAudience"],
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JwtSettings")["SigningKey"]!)),
		ClockSkew = TimeSpan.Zero
	};
	options.Events = new JwtBearerEvents
	{
		OnMessageReceived = context =>
		{
			var accessToken = context.Request.Query["access_token"];
			var path = context.HttpContext.Request.Path;

			if (!string.IsNullOrWhiteSpace(accessToken) && (path.StartsWithSegments("/rooms") || path.StartsWithSegments("/games")))
			{
				context.Token = accessToken;
			}

			if (string.IsNullOrWhiteSpace(accessToken))
			{
				accessToken = context.Request.Headers["Authorization"]
				.ToString()
				.Replace("Bearer ", "");

				context.Token = accessToken;
			}

			return Task.CompletedTask;
		}
	};
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<RoomHub>("/rooms");

app.MapHub<GameHub>("/games");

app.Run();
