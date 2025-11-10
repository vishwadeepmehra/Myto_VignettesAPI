using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.AppModel.ResponseModel
{
    public class ResponseDetail
    {
        public string? Message { get; set; }
        public int? DataLength { get; set; }
        public HttpStatusCode? StatusCode { get; set; }
        public Object? Data { get; set; }
        public bool? IsError { get; set; }
        public Object? ErrorDetail { get; set; }

    }
}
