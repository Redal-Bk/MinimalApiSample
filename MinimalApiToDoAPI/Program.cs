using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinimalApiToDoAPI.AuthEndpoints;
using MinimalApiToDoAPI.Entities;
using MinimalApiToDoAPI.GameEndpoints;
using MinimalApiToDoAPI.SecuritySchema;
using MinimalApiToDoAPI.Services;
using MinimalApiToDoAPI.UserEndpoints;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MinimalContext>(o => o.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});
builder.Services.AddScoped<IUserService,UserService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddAuthorization();
var app = builder.Build();

app.MapOpenApi();


app.MapScalarApiReference(options =>
{
    options
        .WithTitle("My Minimal API")
        .WithTheme(ScalarTheme.BluePlanet)
        .AddPreferredSecuritySchemes("Bearer");

});

app.MapGet("/", () => Results.Redirect("/scalar/v1"));
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapUserEndpoints();
app.MapGameEndpoints();

app.Run();
