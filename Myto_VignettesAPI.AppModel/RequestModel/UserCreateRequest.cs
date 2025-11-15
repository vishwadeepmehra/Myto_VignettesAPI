using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.AppModel.RequestModel
{
    public class UserCreateRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Mobile { get; set; }
        public string? Password { get; set; }
        public string? PreferredLanguage { get; set; }
        public string Role { get; set; } = "Customer";
    }

}
