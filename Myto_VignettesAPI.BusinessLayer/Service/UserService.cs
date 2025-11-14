using Myto_VignettesAPI.AppModel.RequestModel;
using Myto_VignettesAPI.AppModel.ResponseModel;
using Myto_VignettesAPI.BusinessLayer.IService;
using Myto_VignettesAPI.DataLayer;
using Myto_VignettesAPI.DataLayer.AppDbContext;
using System.Net;

namespace Myto_VignettesAPI.BusinessLayer.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseDetail> CreateAsync(UserCreateRequest user)
        {
            var response = await _unitOfWork.Users.CreateAsync(user);

            if (!response.IsError.GetValueOrDefault(false))
                await _unitOfWork.SaveChangesAsync();

            return response;
        }
        public async Task<ResponseDetail> DeleteAsync(long id)
        {
            var response = await _unitOfWork.Users.DeleteAsync(id);

            if (!response.IsError.GetValueOrDefault(false))
                await _unitOfWork.SaveChangesAsync();

            return response;
        }
        public async Task<ResponseDetail> ExistsByEmailAsync(string email)
        {
            return await _unitOfWork.Users.ExistsByEmailAsync(email);
        }
        public async Task<ResponseDetail> GetAllAsync(int pageIndex = 0, int pageSize = 20)
        {
            return await _unitOfWork.Users.GetAllAsync(pageIndex, pageSize);
        }
        public async Task<ResponseDetail> GetByIdAsync(long id)
        {
            return await _unitOfWork.Users.GetByIdAsync(id);
        }
        public async Task<ResponseDetail> UpdateAsync(User user)
        {
            var response = await _unitOfWork.Users.UpdateAsync(user);

            if (!response.IsError.GetValueOrDefault(false))
                await _unitOfWork.SaveChangesAsync();

            return response;
        }
    }
}
