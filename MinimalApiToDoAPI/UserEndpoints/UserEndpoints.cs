using MinimalApiToDoAPI.Models;
using MinimalApiToDoAPI.Services;

namespace MinimalApiToDoAPI.UserEndpoints
{
    public static class UserEndpoints
    {
        public static void MapUserEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/user")
                           .WithTags("Users");
                           //.RequireAuthorization();


            group.MapPost("/", AddUser)
                 .WithSummary("اضافه کردن کاربر جدید");

            group.MapGet("/", GetAllUsers)
                 .WithSummary("دریافت لیست کاربران");
        }

        private static async Task<IResult> AddUser(UserDTO dto, IUserService svc, CancellationToken ct = default)
        {
            var result = await svc.AddUser(dto, ct);
            return result.success ? Results.Ok(result) : Results.BadRequest(result);
        }

        private static async Task<IResult> GetAllUsers(IUserService svc, CancellationToken ct = default)
        {
            return Results.Ok(await svc.AllAsync(ct));
        }
    }
}
