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
                // Null validation
                if (model == null)
                {
                    response.Message = "Invalid request.";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsError = true;
                    return response;
                }

                // Email validation
                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    response.Message = "Email is required.";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsError = true;
                    return response;
                }

                // Normalize role (default = user)
                string role = string.IsNullOrWhiteSpace(model.Role) ? "user" : model.Role.Trim().ToLower();

                // Case-insensitive role validation
                if (!string.Equals(role, "admin", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(role, "user", StringComparison.OrdinalIgnoreCase))
                {
                    response.Message = "Invalid role. Allowed: admin, user";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsError = true;
                    return response;
                }

                // Password validation logic
                bool isInvited = false;

                if (role.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(model.Password))
                    {
                        response.Message = "Password is required for Admin registration.";
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.IsError = true;
                        return response;
                    }
                }
                else if (role.Equals("user", StringComparison.OrdinalIgnoreCase))
                {
                    if (string.IsNullOrWhiteSpace(model.Password))
                    {
                        // Added by Admin → invited customer
                        isInvited = true;
                    }
                }

                // Check if email exists (case-insensitive)
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == model.Email.ToLower());

                if (existingUser != null)
                {
                    response.Message = "Email already registered.";
                    response.StatusCode = HttpStatusCode.Conflict;
                    response.IsError = true;
                    return response;
                }

                // Password hashing (only if password exists)
                string? hashedPassword = null;
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    hashedPassword = ToSHA256(model.Password);
                }

                // Create user entity
                var user = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    PreferredLanguage = model.PreferredLanguage,
                    PasswordHash = hashedPassword,
                    Role = role.ToLower(),
                    IsInvited = isInvited,
                    IsEmailVerified = !isInvited,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                response.Message = "User created successfully.";
                response.StatusCode = HttpStatusCode.Created;
                response.IsError = false;
                response.DataLength = 1;
                response.Data = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Role,
                    user.IsInvited
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
                var query = _context.Users
                    .Include(u => u.Vehicles)
                    .OrderByDescending(u => u.Id)
                    .AsNoTracking();

                int totalRecords = await query.CountAsync();

                List<User> users;

                if (pageSize <= 0)
                {
                    users = await query.ToListAsync();
                }
                else
                {
                    users = await query
                        .Skip(pageIndex * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
                }

                var result = users.Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Mobile,
                    u.PreferredLanguage,
                    u.Role,
                    u.IsInvited,
                    u.IsEmailVerified,
                    u.CreatedAt,
                    u.UpdatedAt,

                    Vehicles = u.Vehicles.Select(v => new
                    {
                        v.Id,
                        v.UserId,
                        v.CountryCode,
                        v.RegistrationNumber,
                        v.VehicleCategory,
                        v.CreatedAt,
                        v.UpdatedAt
                    }).ToList()
                }).ToList();

                response.Message = "Users fetched";
                response.StatusCode = HttpStatusCode.OK;
                response.IsError = false;
                response.Data = result;
                response.DataLength = result.Count;
                response.TotalRecords = totalRecords;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.Message = "Error fetching users";
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
        public async Task<ResponseDetail> GetAllByUserIdAsync(long userId, int pageIndex = 0, int pageSize = 20)
        {
            var response = new ResponseDetail();

            try
            {
                IQueryable<Vehicle> query = _context.Vehicles
                    .AsNoTracking()
                    .Where(v => v.UserId == userId)
                    .OrderByDescending(v => v.CreatedAt);

                List<Vehicle> vehicles;

                if (pageSize <= 0)
                {
                    vehicles = await query.ToListAsync(); // return all
                }
                else
                {
                    vehicles = await query
                        .Skip(pageIndex * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
                }

                var result = vehicles.Select(v => new
                {
                    v.Id,
                    v.UserId,
                    v.CountryCode,
                    v.RegistrationNumber,
                    v.VehicleCategory,
                    v.CreatedAt,
                    v.UpdatedAt
                }).ToList();

                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Vehicles fetched successfully.";
                response.Data = result;
                response.DataLength = result.Count;
                response.IsError = false;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Error fetching vehicles.";
                response.ErrorDetail = ex.Message;
                response.IsError = true;
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
