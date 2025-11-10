using Myto_VignettesAPI.AppModel.ResponseModel;
using Myto_VignettesAPI.DataLayer.AppDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.BusinessLayer.IService
{
    public interface IUserService
    {
        Task<ResponseDetail> CreateAsync(User user);

        Task<ResponseDetail> GetByIdAsync(long id);

        Task<ResponseDetail> GetAllAsync(int pageIndex = 0, int pageSize = 20);

        Task<ResponseDetail> UpdateAsync(User user);

        Task<ResponseDetail> DeleteAsync(long id);

        Task<ResponseDetail> ExistsByEmailAsync(string email);
    }
}
