using Myto_VignettesAPI.AppModel.RequestModel;
using Myto_VignettesAPI.AppModel.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.DataLayer.DataInterface
{
    public interface IAuthRepository
    {
        Task<ResponseDetail> LoginAsync(LoginRequest loginRequest);
    }
}
