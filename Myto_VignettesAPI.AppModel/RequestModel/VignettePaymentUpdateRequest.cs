    using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.AppModel.RequestModel
{
    public class VignettePaymentUpdateRequest
    {
        public long PurchaseId { get; set; }
        public string PaymentStatus { get; set; } = "SUCCESS";
        public string PaymentTransactionId { get; set; } = string.Empty;
        public string? ReceiptPdfUrl { get; set; }
    }

}
