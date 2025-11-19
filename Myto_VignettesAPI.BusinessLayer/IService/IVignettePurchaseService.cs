using Myto_VignettesAPI.AppModel.RequestModel;
using Myto_VignettesAPI.AppModel.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.BusinessLayer.IService
{
    public interface IVignettePurchaseService
    {
        Task<ResponseDetail> CreateAsync(VignettePurchaseCreateRequest model);
        Task<ResponseDetail> GetByIdAsync(long id);
        Task<ResponseDetail> GetByUserAsync(long userId, int pageIndex, int pageSize);
        Task<ResponseDetail> UpdatePaymentAsync(VignettePaymentUpdateRequest model);
    }
}
