using Myto_VignettesAPI.DataLayer.AppDbContext;
using Myto_VignettesAPI.DataLayer.DataInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.DataLayer
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly VignettesContext _context;

        public UnitOfWork(VignettesContext context, IUserRepository users, IAuthRepository auths, IVehicleRepository vehicles, IVignettePurchaseRepository vignettePurchases)
        {
            _context = context;
            Users = users;
            Auths = auths;
            Vehicles = vehicles;
            VignettePurchases = vignettePurchases;
        }

        public IUserRepository Users { get; }
        public IAuthRepository Auths { get; }
        public IVehicleRepository Vehicles { get; }
        public IVignettePurchaseRepository VignettePurchases { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

