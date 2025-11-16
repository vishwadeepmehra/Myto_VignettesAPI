using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.AppModel.RequestModel
{
    public class VehicleCreateRequest
    {
       public long Id { get; set; }
        public long UserId { get; set; }

        [Required(ErrorMessage = "Country code is required.")]
        [StringLength(5, ErrorMessage = "Country code cannot exceed 5 characters.")]
        public string CountryCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Registration number is required.")]
        [StringLength(50, ErrorMessage = "Registration number cannot exceed 50 characters.")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vehicle category is required.")]
        [RegularExpression("^(D1M|D1|D2|U)$", ErrorMessage = "Vehicle category must be one of: D1M, D1, D2, U.")]
        public string VehicleCategory { get; set; } = string.Empty;
    }
}
