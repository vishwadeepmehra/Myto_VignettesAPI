using Microsoft.EntityFrameworkCore;
using Myto_VignettesAPI.AppModel.RequestModel;
using Myto_VignettesAPI.AppModel.ResponseModel;
using Myto_VignettesAPI.DataLayer.AppDbContext;
using Myto_VignettesAPI.DataLayer.DataInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.DataLayer.DataInteraction
{
    public class UserRepository:IUserRepository
    {
        private readonly VignettesContext _context;

        public UserRepository(VignettesContext context)
        {
            _context = context;
        }
        public async Task<ResponseDetail> CreateAsync(UserCreateRequest model)
        {
            var response = new ResponseDetail();

            try
            {
                // ✅ Validate input
                if (model == null || string.IsNullOrWhiteSpace(model.Password))
                {
                    response.Message = "User details or password cannot be empty.";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsError = true;
                    return response;
                }

                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    response.Message = "Email is required.";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsError = true;
                    return response;
                }

                // ✅ Check if email already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (existingUser != null)
                {
                    response.Message = "Email already registered.";
                    response.StatusCode = HttpStatusCode.Conflict;
                    response.IsError = true;
                    return response;
                }

                // ✅ Hash password
                var hashedPassword = ToSHA256(model.Password);

                // ✅ Map DTO → Entity
                var user = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    PasswordHash = hashedPassword,
                    PreferredLanguage = model.PreferredLanguage,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // ✅ Save to DB
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                // ✅ Build Response
                response.Message = "User registered successfully.";
                response.StatusCode = HttpStatusCode.Created;
                response.IsError = false;
                response.DataLength = 1;
                response.Data = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Mobile,
                    user.PreferredLanguage
                };
            }
            catch (Exception ex)
            {
                response.Message = "Failed to register user.";
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsError = true;
                response.ErrorDetail = ex.Message;
            }

            return response;
        }
        public async Task<ResponseDetail> GetByIdAsync(long id)
        {
            var response = new ResponseDetail();
            try
            {
                var user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                if (user == null)
                {
                    response.Message = "User not found.";
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsError = true;
                }
                else
                {
                    response.Message = "User retrieved successfully.";
                    response.StatusCode = HttpStatusCode.OK;
                    response.Data = user;
                    response.DataLength = 1;
                    response.IsError = false;
                }
            }
            catch (Exception ex)
            {
                response.Message = "Error fetching user.";
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsError = true;
                response.ErrorDetail = ex.Message;
            }

            return response;
        }
        public async Task<ResponseDetail> GetAllAsync(int pageIndex = 0, int pageSize = 20)
        {
            var response = new ResponseDetail();

            try
            {
                IQueryable<User> query = _context.Users.AsNoTracking().OrderByDescending(u => u.Id);

                List<User> users;

                if (pageSize <= 0)
                {
                    // ✅ Return all users (no pagination)
                    users = await query.ToListAsync();
                }
                else
                {
                    // ✅ Apply pagination normally
                    users = await query
                        .Skip(pageIndex * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
                }

                response.Message = pageSize <= 0
                    ? "All users retrieved successfully."
                    : "Paged user list retrieved successfully.";

                response.StatusCode = HttpStatusCode.OK;
                response.Data = users;
                response.DataLength = users.Count;
                response.IsError = false;
            }
            catch (Exception ex)
            {
                response.Message = "Error fetching users.";
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsError = true;
                response.ErrorDetail = ex.Message;
            }

            return response;
        }
        public async Task<ResponseDetail> UpdateAsync(User user)
        {
            var response = new ResponseDetail();
            try
            {
                var existing = await _context.Users.FindAsync(user.Id);
                if (existing == null)
                {
                    response.Message = "User not found.";
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsError = true;
                    return response;
                }

                existing.Name = user.Name;
                existing.Email = user.Email;
                existing.Mobile = user.Mobile;
                existing.PasswordHash = user.PasswordHash;
                existing.PreferredLanguage = user.PreferredLanguage;
                existing.IsEmailVerified = user.IsEmailVerified;
                existing.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                response.Message = "User updated successfully.";
                response.StatusCode = HttpStatusCode.OK;
                response.Data = existing;
                response.DataLength = 1;
                response.IsError = false;
            }
            catch (Exception ex)
            {
                response.Message = "Error updating user.";
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsError = true;
                response.ErrorDetail = ex.Message;
            }

            return response;
        }
        public async Task<ResponseDetail> DeleteAsync(long id)
        {
            var response = new ResponseDetail();
            try
            {
                var existing = await _context.Users.FindAsync(id);
                if (existing == null)
                {
                    response.Message = "User not found.";
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.IsError = true;
                    return response;
                }

                _context.Users.Remove(existing);
                await _context.SaveChangesAsync();

                response.Message = "User deleted successfully.";
                response.StatusCode = HttpStatusCode.OK;
                response.IsError = false;
                response.Data = null;
                response.DataLength = 0;
            }
            catch (Exception ex)
            {
                response.Message = "Error deleting user.";
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsError = true;
                response.ErrorDetail = ex.Message;
            }

            return response;
        }
        public async Task<ResponseDetail> ExistsByEmailAsync(string email)
        {
            var response = new ResponseDetail();
            try
            {
                var exists = await _context.Users.AnyAsync(u => u.Email == email);

                response.Message = exists
                    ? "Email already exists."
                    : "Email is available.";

                response.StatusCode = HttpStatusCode.OK;
                response.IsError = false;
                response.Data = new { Exists = exists };
                response.DataLength = 1;
            }
            catch (Exception ex)
            {
                response.Message = "Error checking email existence.";
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsError = true;
                response.ErrorDetail = ex.Message;
            }

            return response;
        }

        public static string ToSHA256(string s)
        {
            try
            {
                using var sha256 = SHA256.Create();
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(s));

                var sb = new StringBuilder();
                foreach (var b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
            catch
            {
                throw;
            }
        }

    }
}
