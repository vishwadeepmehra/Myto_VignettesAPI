using Myto_VignettesAPI.AppModel.RequestModel;
using Myto_VignettesAPI.AppModel.ResponseModel;
using Myto_VignettesAPI.BusinessLayer.IService;
using Myto_VignettesAPI.DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.BusinessLayer.Service
{
    public class VignettePurchaseService : IVignettePurchaseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VignettePurchaseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<ResponseDetail> CreateAsync(VignettePurchaseCreateRequest request)
            => _unitOfWork.VignettePurchases.CreateAsync(request);

        public Task<ResponseDetail> GetByIdAsync(long id)
            => _unitOfWork.VignettePurchases.GetByIdAsync(id);

        public Task<ResponseDetail> GetByUserAsync(long userId, int pageIndex, int pageSize)
            => _unitOfWork.VignettePurchases.GetByUserAsync(userId, pageIndex, pageSize);

        public Task<ResponseDetail> UpdatePaymentAsync(VignettePaymentUpdateRequest model)
            => _unitOfWork.VignettePurchases.UpdatePaymentAsync(model);
    }

}
