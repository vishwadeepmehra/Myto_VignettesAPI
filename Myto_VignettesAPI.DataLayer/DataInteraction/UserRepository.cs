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
                // Basic null validation
                if (model == null)
                {
                    response.Message = "Invalid request.";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsError = true;
                    return response;
                }

                // Email is mandatory for both Admin + Customer
                if (string.IsNullOrWhiteSpace(model.Email))
                {
                    response.Message = "Email is required.";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsError = true;
                    return response;
                }

                // Normalize role
                string role = string.IsNullOrWhiteSpace(model.Role) ? "Customer" : model.Role;

                if (role != "Admin" && role != "Customer")
                {
                    response.Message = "Invalid role. Allowed: Admin, Customer";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsError = true;
                    return response;
                }

                // Validate password only for Admin OR customer self-registration
                bool isInvited = false;

                if (role == "Admin")
                {
                    if (string.IsNullOrWhiteSpace(model.Password))
                    {
                        response.Message = "Password is required for Admin registration.";
                        response.StatusCode = HttpStatusCode.BadRequest;
                        response.IsError = true;
                        return response;
                    }
                }
                else if (role == "Customer")
                {
                    // Customer added by Admin → no password required
                    if (string.IsNullOrWhiteSpace(model.Password))
                    {
                        isInvited = true;  // Added by Admin, activation later
                    }
                }

                // Check if email exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == model.Email);

                if (existingUser != null)
                {
                    response.Message = "Email already registered.";
                    response.StatusCode = HttpStatusCode.Conflict;
                    response.IsError = true;
                    return response;
                }

                // Hash only if password exists
                string? hashedPassword = null;
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    hashedPassword = ToSHA256(model.Password);
                }

                // Map DTO → Entity
                var user = new User
                {
                    Name = model.Name,
                    Email = model.Email,
                    Mobile = model.Mobile,
                    PreferredLanguage = model.PreferredLanguage,
                    PasswordHash = hashedPassword,
                    IsEmailVerified = !isInvited,   // invited customers are not verified
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Role = role,
                    IsInvited = isInvited
                };

                // Save 
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                // Response
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
                // Base query including vehicles but avoiding tracking
                IQueryable<User> query = _context.Users
                    .AsNoTracking()
                    .Include(u => u.Vehicles)
                    .OrderByDescending(u => u.Id);

                List<User> users;

                // If pageSize = 0 → return all users
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

                // Map to DTO to avoid exposing password, unwanted fields
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

                response.Message = pageSize <= 0
                    ? "All users retrieved successfully."
                    : "Paged user list retrieved successfully.";

                response.StatusCode = HttpStatusCode.OK;
                response.Data = result;
                response.DataLength = result.Count;
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
