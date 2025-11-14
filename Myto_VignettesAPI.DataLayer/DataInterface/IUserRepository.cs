using Myto_VignettesAPI.AppModel.RequestModel;
using Myto_VignettesAPI.AppModel.ResponseModel;
using Myto_VignettesAPI.DataLayer.AppDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.DataLayer.DataInterface
{
    public interface IUserRepository
    {
        Task<ResponseDetail> CreateAsync(UserCreateRequest user);

        Task<ResponseDetail> GetByIdAsync(long id);

        Task<ResponseDetail> GetAllAsync(int pageIndex = 0, int pageSize = 20);

        Task<ResponseDetail> UpdateAsync(User user);

        Task<ResponseDetail> DeleteAsync(long id);

        Task<ResponseDetail> ExistsByEmailAsync(string email);
    }
}
