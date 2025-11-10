using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Myto_VignettesAPI.AppModel.RequestModel;
using Myto_VignettesAPI.AppModel.ResponseModel;
using Myto_VignettesAPI.DataLayer.AppDbContext;
using Myto_VignettesAPI.DataLayer.DataInterface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.DataLayer.DataInteraction
{
    public class AuthRepository :IAuthRepository
    {
        private readonly IConfiguration _configuration;
        private readonly VignettesContext _context;

        public AuthRepository(IConfiguration configuration, VignettesContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        /// <summary>
        /// Handles user login and returns JWT if successful.
        /// </summary>
        public async Task<ResponseDetail> LoginAsync(LoginRequest loginRequest)
        {
            var response = new ResponseDetail();

            try
            {
                // 1️⃣ Fetch user by email
                var user = _context.Users.FirstOrDefault(u => u.Email == loginRequest.Email);
                if (user == null)
                {
                    response.Message = "Invalid email or password.";
                    response.IsError = true;
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    return response;
                }

                // 2️⃣ Verify password using SHA256
                var hashedInputPassword = ToSHA256(loginRequest.Password);

                if (!string.Equals(hashedInputPassword, user.PasswordHash, StringComparison.OrdinalIgnoreCase))
                {
                    response.Message = "Invalid email or password.";
                    response.IsError = true;
                    response.StatusCode = HttpStatusCode.Unauthorized;
                    return response;
                }

                // 3️⃣ Optional: Check if email is verified
                if (user.IsEmailVerified == false)
                {
                    response.Message = "Email not verified.";
                    response.IsError = true;
                    response.StatusCode = HttpStatusCode.Forbidden;
                    return response;
                }

                // 4️⃣ Build claims
                var claims = new List<Claim>
                {
                    new Claim("UserId", user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Name ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                // Add Role (if implemented later)
                var roleName = "User";
                claims.Add(new Claim(ClaimTypes.Role, roleName));

                // 5️⃣ Create token
                var token = CreateToken(claims);
                var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);

                var tokenDetails = new
                {
                    Token = tokenValue,
                    Expiration = token.ValidTo
                };

                response.Message = "Login successful.";
                response.IsError = false;
                response.StatusCode = HttpStatusCode.OK;
                response.Data = tokenDetails;
                response.DataLength = 1;
            }
            catch (Exception ex)
            {
                response.Message = "An error occurred during authentication.";
                response.IsError = true;
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.ErrorDetail = ex.Message;
            }

            return response;
        }

        /// <summary>
        /// Creates a JWT access token.
        /// </summary>
        private JwtSecurityToken CreateToken(IEnumerable<Claim> claims)
        {
            var key = _configuration["Jwt:Key"]
                      ?? throw new InvalidOperationException("JWT key not configured in appsettings.json.");

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var expiryMinutes = 240;
            if (int.TryParse(_configuration["Jwt:ExpiryMinutes"], out var configuredMinutes))
                expiryMinutes = configuredMinutes;

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return token;
        }

        /// <summary>
        /// Converts a plain string into SHA256 hash (hexadecimal string).
        /// </summary>
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
