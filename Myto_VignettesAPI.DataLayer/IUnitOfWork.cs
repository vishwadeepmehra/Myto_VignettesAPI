using Myto_VignettesAPI.DataLayer.DataInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.DataLayer
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        IAuthRepository Auths { get; }

        Task<int> SaveChangesAsync();    
}
}
