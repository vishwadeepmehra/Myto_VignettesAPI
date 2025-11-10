using Myto_VignettesAPI.AppModel.RequestModel;
using Myto_VignettesAPI.AppModel.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.BusinessLayer.IService
{
    public interface IAuthService 
    {
        Task<ResponseDetail> LoginAsync(LoginRequest loginRequest);
    }
}
