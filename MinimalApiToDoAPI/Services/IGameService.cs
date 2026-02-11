using Microsoft.EntityFrameworkCore;
using MinimalApiToDoAPI.Entities;
using MinimalApiToDoAPI.Models;
using System.Linq;

namespace MinimalApiToDoAPI.Services
{
    public interface IGameService
    {
        Task<ResponseModel> AddAsync(GameDTO dto , CancellationToken ct = default);
        Task<ResponseModel<List<GameDTO>>> AllAsync(CancellationToken ct = default);
    }
    public class GameService : IGameService
    {
        private readonly MinimalContext _db;
        public GameService(MinimalContext db)
        {
            _db = db;
        }

        public async Task<ResponseModel> AddAsync(GameDTO dto, CancellationToken ct = default)
        {
            var response = new ResponseModel() { message = "done.", success = true };
            try
            {
                if(string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Publisher))
                {
                    response.success = false;
                    response.message = "fill correct details.";
                    return response;
                }
                var data = new Game
                {
                    Title = dto.Name,
                    Publisher = dto.Publisher,
                    ReleaseDate = dto.ReleaseDate,
                };
                await _db.Games.AddAsync(data);
                await _db.SaveChangesAsync();
                return response;
            }
            catch(Exception ex)
            {
                response.success = false;
                response.message = ex.Message;
                return response;
            }
        }

        public async Task<ResponseModel<List<GameDTO>>> AllAsync(CancellationToken ct = default)
        {
            var res = new ResponseModel<List<GameDTO>>() { message = "done." , success = true};
            try
            {
                res.response = await _db.Games.AsNoTracking()
                    .OrderByDescending(x => x.Id)
                    .Select(x => new GameDTO
                    {
                        Name = x.Title ?? "",
                        Publisher = x.Publisher ?? "",
                        ReleaseDate = x.ReleaseDate,

                    })
                    .ToListAsync(ct);
                return res;
            }
            catch(Exception ex)
            {
                res.success = false;
                res.message = ex.Message;
                return res;
            }
        }
    }
}
