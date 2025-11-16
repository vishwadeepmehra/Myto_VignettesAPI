using Myto_VignettesAPI.AppModel.RequestModel;
using Myto_VignettesAPI.AppModel.ResponseModel;
using Myto_VignettesAPI.BusinessLayer.IService;
using Myto_VignettesAPI.DataLayer;
using Myto_VignettesAPI.DataLayer.AppDbContext;
using System.Net;

namespace Myto_VignettesAPI.BusinessLayer.Service
{
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VehicleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDetail> CreateAsync(VehicleCreateRequest vehicle)
        {
            var response = new ResponseDetail();
            try
            {
                if (vehicle == null)
                {
                    response.Message = "Invalid vehicle data.";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsError = true;
                    return response;
                }

                // Additional validations if needed
                if (string.IsNullOrWhiteSpace(vehicle.RegistrationNumber))
                {
                    response.Message = "Vehicle registration number is required.";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsError = true;
                    return response;
                }

                response = await _unitOfWork.Vehicles.CreateAsync(vehicle);
            }
            catch (Exception ex)
            {
                response.Message = "Error while creating vehicle.";
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsError = true;
                response.ErrorDetail = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDetail> UpdateAsync(VehicleCreateRequest vehicle)
        {
            var response = new ResponseDetail();
            try
            {
                if (vehicle.Id <= 0)
                {
                    response.Message = "Vehicle ID is required for update.";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsError = true;
                    return response;
                }

                response = await _unitOfWork.Vehicles.UpdateAsync(vehicle);
            }
            catch (Exception ex)
            {
                response.Message = "Error while updating vehicle.";
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
                if (id <= 0)
                {
                    response.Message = "Invalid vehicle ID.";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsError = true;
                    return response;
                }

                response = await _unitOfWork.Vehicles.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error while deleting vehicle.";
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
                if (id <= 0)
                {
                    response.Message = "Invalid vehicle ID.";
                    response.StatusCode = HttpStatusCode.BadRequest;
                    response.IsError = true;
                    return response;
                }

                response = await _unitOfWork.Vehicles.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                response.Message = "Error while fetching vehicle details.";
                response.StatusCode = HttpStatusCode.InternalServerError;
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


                response = await _unitOfWork.Vehicles.GetAllByUserAsync(pageIndex, pageSize);
            }
            catch (Exception ex)
            {
                response.Message = "Error while fetching user vehicles.";
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.IsError = true;
                response.ErrorDetail = ex.Message;
            }

            return response;
        }

        public async Task<ResponseDetail> GetByRegistrationNumberAsync(string registrationNumber)
        {
            return await _unitOfWork.Vehicles.GetByRegistrationNumberAsync(registrationNumber);
        }
    }
}
