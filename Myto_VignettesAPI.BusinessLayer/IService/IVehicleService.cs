using Myto_VignettesAPI.AppModel.RequestModel;
using Myto_VignettesAPI.AppModel.ResponseModel;
using Myto_VignettesAPI.DataLayer.AppDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.BusinessLayer.IService
{
    public interface IVehicleService
    {
        Task<ResponseDetail> CreateAsync(VehicleCreateRequest vehicle);
        Task<ResponseDetail> UpdateAsync(VehicleCreateRequest vehicle);
        Task<ResponseDetail> DeleteAsync(long id);
        Task<ResponseDetail> GetByIdAsync(long id);
        Task<ResponseDetail> GetAllByUserAsync(int pageIndex = 0, int pageSize = 20);
    }
}
