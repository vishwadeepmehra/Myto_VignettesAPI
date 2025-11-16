using Microsoft.EntityFrameworkCore;
using Myto_VignettesAPI.AppModel.RequestModel;
using Myto_VignettesAPI.AppModel.ResponseModel;
using Myto_VignettesAPI.DataLayer.AppDbContext;
using Myto_VignettesAPI.DataLayer.DataInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.DataLayer.DataInteraction
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly VignettesContext _context;

        public VehicleRepository(VignettesContext context)
        {
            _context = context;
        }

        public async Task<ResponseDetail> CreateAsync(VehicleCreateRequest model)
        {
            var response = new ResponseDetail();
            try
            {
                // Prevent duplicate registration numbers for same user
                var existing = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.UserId == model.UserId && v.RegistrationNumber == model.RegistrationNumber);

                if (existing != null)
                {
                    response.StatusCode = HttpStatusCode.Conflict;
                    response.Message = "A vehicle with this registration number already exists for this user.";
                    response.IsError = true;
                    return response;
                }

                // Map DTO → Entity
                var vehicle = new Vehicle
                {
                    UserId = model.UserId,
                    CountryCode = model.CountryCode,
                    RegistrationNumber = model.RegistrationNumber,
                    VehicleCategory = model.VehicleCategory,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.Vehicles.AddAsync(vehicle);
                await _context.SaveChangesAsync();

                response.StatusCode = HttpStatusCode.Created;
                response.Message = "Vehicle created successfully.";
                response.Data = vehicle;
                response.DataLength = 1;
                response.IsError = false;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Error creating vehicle.";
                response.IsError = true;
                response.ErrorDetail = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDetail> UpdateAsync(VehicleCreateRequest model)
        {
            var response = new ResponseDetail();
            try
            {
                var existing = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == model.Id);
                if (existing == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "Vehicle not found.";
                    response.IsError = true;
                    return response;
                }

                // Optional: prevent duplicate registration numbers for same user
                var duplicate = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.RegistrationNumber == model.RegistrationNumber &&
                                              v.UserId == existing.UserId);
                if (duplicate != null)
                {
                    response.StatusCode = HttpStatusCode.Conflict;
                    response.Message = "Another vehicle with this registration number already exists for this user.";
                    response.IsError = true;
                    return response;
                }

                // Update allowed fields only
                existing.CountryCode = model.CountryCode;
                existing.RegistrationNumber = model.RegistrationNumber;
                existing.VehicleCategory = model.VehicleCategory;
                existing.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Vehicle updated successfully.";
                response.Data = existing;
                response.DataLength = 1;
                response.IsError = false;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Error updating vehicle.";
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
                var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == id);
                if (vehicle == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "Vehicle not found.";
                    response.IsError = true;
                    return response;
                }

                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();

                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Vehicle deleted successfully.";
                response.Data = vehicle;
                response.DataLength = 1;
                response.IsError = false;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Error deleting vehicle.";
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
                var vehicle = await _context.Vehicles
                    .FirstOrDefaultAsync(v => v.Id == id);

                if (vehicle == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "Vehicle not found.";
                    response.IsError = true;
                    return response;
                }

                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Vehicle fetched successfully.";
                response.Data = vehicle;
                response.DataLength = 1;
                response.IsError = false;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Error fetching vehicle.";
                response.IsError = true;
                response.ErrorDetail = ex.Message;
            }
            return response;
        }

        public async Task<ResponseDetail> GetAllByUserAsync(int pageIndex = 0, int pageSize = 20)
        {
            var response = new ResponseDetail();

            try
            {
                IQueryable<Vehicle> query = _context.Vehicles
                    .AsNoTracking()
                    .Include(v => v.User)                // Include user details
                    .OrderByDescending(v => v.CreatedAt);

                List<Vehicle> vehicles;

                // Return all data if pageSize = 0
                if (pageSize <= 0)
                {
                    vehicles = await query.ToListAsync();
                }
                else
                {
                    vehicles = await query
                        .Skip(pageIndex * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
                }

                // Shape output
                var result = vehicles.Select(v => new
                {
                    v.Id,
                    v.UserId,
                    v.CountryCode,
                    v.RegistrationNumber,
                    v.VehicleCategory,
                    v.CreatedAt,
                    v.UpdatedAt,

                    User = v.User == null ? null : new
                    {
                        v.User.Id,
                        v.User.Name,
                        v.User.Email,
                        v.User.Mobile,
                        v.User.Role,
                        v.User.IsInvited
                    }
                }).ToList();

                response.StatusCode = HttpStatusCode.OK;
                response.Message = pageSize <= 0
                    ? "All vehicles retrieved successfully."
                    : "Paged vehicle list retrieved successfully.";

                response.Data = result;
                response.DataLength = result.Count;
                response.IsError = false;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Error fetching vehicles.";
                response.IsError = true;
                response.ErrorDetail = ex.Message;
            }

            return response;
        }


        public async Task<ResponseDetail> GetByRegistrationNumberAsync(string registrationNumber)
        {
            var response = new ResponseDetail();

            try
            {
                var vehicle = await _context.Vehicles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(v => v.RegistrationNumber == registrationNumber);

                if (vehicle == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    response.Message = "Vehicle not found.";
                    response.IsError = true;
                    return response;
                }

                var result = new
                {
                    vehicle.Id,
                    vehicle.UserId,
                    vehicle.CountryCode,
                    vehicle.RegistrationNumber,
                    vehicle.VehicleCategory,
                    vehicle.CreatedAt,
                    vehicle.UpdatedAt
                };

                response.StatusCode = HttpStatusCode.OK;
                response.Message = "Vehicle details fetched successfully.";
                response.Data = result;
                response.DataLength = 1;
                response.IsError = false;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Error fetching vehicle details.";
                response.ErrorDetail = ex.Message;
                response.IsError = true;
            }

            return response;
        }

    }
}
