using MinimalApiToDoAPI.Models;
using MinimalApiToDoAPI.Services;

namespace MinimalApiToDoAPI.GameEndpoints
{
    public static class GameEndpoints
    {
        public static void MapGameEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/game")
                           .WithTags("Games")
                           .RequireAuthorization();

            group.MapPost("/", AddGame)
                 .WithSummary("اضافه کردن بازی جدید")
                 .WithDescription("یک بازی جدید با عنوان، ناشر و تاریخ انتشار به دیتابیس اضافه می‌کند.")
                 .Produces(StatusCodes.Status200OK)
                 .Produces(StatusCodes.Status400BadRequest);

            group.MapGet("/", GetAllGames)
                 .WithSummary("دریافت لیست همه بازی‌ها")
                 .WithDescription("تمام بازی‌های موجود در دیتابیس را برمی‌گرداند.")
                 .Produces<ResponseModel<List<GameDTO>>>(StatusCodes.Status200OK);
        }

        private static async Task<IResult> AddGame(GameDTO dto, IGameService service, CancellationToken ct = default)
        {
            var result = await service.AddAsync(dto, ct);

            if (result.success)
            {
                return Results.Ok(result);  
            }

            return Results.BadRequest(result);
        }

        private static async Task<IResult> GetAllGames(IGameService service, CancellationToken ct = default)
        {
            var result = await service.AllAsync(ct);
            return Results.Ok(result);
        }
    }
}
