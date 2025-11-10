using Myto_VignettesAPI.AppModel.RequestModel;
using Myto_VignettesAPI.AppModel.ResponseModel;
using Myto_VignettesAPI.BusinessLayer.IService;
using Myto_VignettesAPI.DataLayer;
using Myto_VignettesAPI.DataLayer.AppDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.BusinessLayer.Service
{
    public class AuthService :IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ResponseDetail> LoginAsync(LoginRequest loginRequest)
        {
            var response = await _unitOfWork.Auths.LoginAsync(loginRequest);

            if (!response.IsError.GetValueOrDefault(false))
                await _unitOfWork.SaveChangesAsync();

            return response;
        }

    }
}
