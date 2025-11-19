using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.AppModel.RequestModel
{
    public class VignettePurchaseCreateRequest
    {
        public long UserId { get; set; }
        public long VehicleId { get; set; }
        public string CountryCode { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string VehicleCategory { get; set; } = string.Empty;

        public string VignetteType { get; set; } = string.Empty;

        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }

        public decimal BasePrice { get; set; }
        public decimal Commission { get; set; }
    }

}
