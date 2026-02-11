using Microsoft.IdentityModel.Tokens;
using MinimalApiToDoAPI.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MinimalApiToDoAPI.AuthEndpoints
{
    public static class AuthEndpoints
    {
        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/auth")
                           .WithTags("Auth");

            group.MapPost("/login", Login)
                 .WithSummary("ورود کاربر و دریافت JWT")
                 .WithDescription("با username و password معتبر، توکن JWT برمی‌گرداند.")
                 .Produces<AuthResponse>(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status401Unauthorized);
        }

        private static async Task<IResult> Login(LoginDTO dto, IUserService userService, IConfiguration config)
        {
            
            var user = await userService.ValidateUserAsync(dto.Username, dto.Password);

            if (user == null)
            {
                return Results.Unauthorized();
            }

            var jwtSettings = config.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);
            var expires = DateTime.UtcNow.AddMinutes(int.Parse(jwtSettings["ExpiresInMinutes"] ?? "60"));

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? "0"),
               
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Results.Ok(new AuthResponse(jwt, expires));
        }
    }
}