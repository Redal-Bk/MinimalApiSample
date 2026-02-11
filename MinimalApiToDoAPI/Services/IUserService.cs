using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MinimalApiToDoAPI.Entities;
using MinimalApiToDoAPI.Models;


namespace MinimalApiToDoAPI.Services
{
    public interface IUserService
    {
        Task<ResponseModel<List<UserDTO>>> AllAsync(CancellationToken ct = default);
        Task<ResponseModel> AddUser(UserDTO dto , CancellationToken ct = default);
        Task<User?> ValidateUserAsync(string username, string password, CancellationToken ct = default);
    }        
    public class UserService : IUserService
    {
        private readonly MinimalContext _db;
        private readonly IPasswordHasher<User> _hash;
        public UserService(MinimalContext db, IPasswordHasher<User> hash)
        {
            _db = db;
            _hash = hash;
        }

        public async Task<User?> ValidateUserAsync(string username, string password, CancellationToken ct = default)
        {
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username, ct);

            if (user == null)
            {
                return null;  
            }

           
            var verificationResult = _hash.VerifyHashedPassword(
                user,
                user.Password,   
                password             
            );

            if (verificationResult == PasswordVerificationResult.Success ||
                verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {

                if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    user.Password = _hash.HashPassword(user, password);
                    _db.Users.Update(user);
                    await _db.SaveChangesAsync(ct);
                }

                return user;
            }

            return null;  
        }
        public async Task<ResponseModel<List<UserDTO>>> AllAsync(CancellationToken ct = default)
        {
            var res = new ResponseModel<List<UserDTO>>() { message = "done.", success = true };
            try
            {
                res.response = await _db.Users.AsNoTracking()
                .OrderByDescending(x => x.Id)
                .Select(x => new UserDTO
                {
                    Username = x.Username ?? "",
                    Password = x.Password ?? ""
                })
                .ToListAsync(ct);
                return res;
            }
            catch(Exception ex)
            {
                res.message = ex.Message;   
                res.success = false;
                return res;
            }
                            
        }

        public async Task<ResponseModel> AddUser(UserDTO dto, CancellationToken ct = default)
        {
            var res = new ResponseModel() { message = "done.", success = true };
            try
            {
                if(string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
                {
                    res.message = "fill correct please";
                    res.success = false;
                    return res;
                }
                
                var user = await _db.Users.FirstOrDefaultAsync(x => x.Username == dto.Username);
                if(user == null)
                {
                    var data = new User
                    {
                        Username = dto.Username,                       
                    };
                    data.Password = _hash.HashPassword(data, dto.Password);
                    await _db.Users.AddAsync(data);
                    await _db.SaveChangesAsync();
                    return res;
                }
                res.success = false;
                res.message = "user already exist.";
                return res;
            }
            catch(Exception ex)
            {
                res.message = ex.Message;
                res.success = false;
                return res;
            }

        }
    }
}
