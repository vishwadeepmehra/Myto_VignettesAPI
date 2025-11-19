using Microsoft.EntityFrameworkCore;
using Myto_VignettesAPI.AppModel.RequestModel;
using Myto_VignettesAPI.AppModel.ResponseModel;
using Myto_VignettesAPI.DataLayer.AppDbContext;
using Myto_VignettesAPI.DataLayer.DataInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Myto_VignettesAPI.DataLayer.DataInteraction
{
    public class VignettePurchaseRepository : IVignettePurchaseRepository
    {
        private readonly VignettesContext _context;

        public VignettePurchaseRepository(VignettesContext context)
        {
            _context = context;
        }

        public async Task<ResponseDetail> CreateAsync(VignettePurchaseCreateRequest model)
        {
            var response = new ResponseDetail();
            try
            {
                var entity = new VignettePurchase
                {
                    UserId = model.UserId,
                    VehicleId = model.VehicleId,
                    CountryCode = model.CountryCode,
                    RegistrationNumber = model.RegistrationNumber,
                    VehicleCategory = model.VehicleCategory,
                    VignetteType = model.VignetteType,
                    ValidityStart = model.ValidityStart,
                    ValidityEnd = model.ValidityEnd,
                    BasePrice = model.BasePrice,
                    Commission = model.Commission,
                    FinalPrice = model.BasePrice + model.Commission,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.VignettePurchases.AddAsync(entity);
                await _context.SaveChangesAsync();

                response.StatusCode = HttpStatusCode.Created;
                response.Message = "Vignette purchase created successfully.";
                response.Data = entity;
                response.IsError = false;
            }
            catch (Exception ex)
            {
                response.StatusCode = HttpStatusCode.InternalServerError;
                response.Message = "Error creating purchase.";
                response.IsError = true;
                response.ErrorDetail = ex.Message;
            }
            return response;
        }

        public async Task<ResponseDetail> GetByIdAsync(long id)
        {
            var data = await _context.VignettePurchases.FindAsync(id);

            return new ResponseDetail
            {
                StatusCode = data != null ? HttpStatusCode.OK : HttpStatusCode.NotFound,
                Message = data != null ? "Purchase found." : "Purchase not found.",
                Data = data,
                DataLength = data != null ? 1 : 0,
                IsError = data == null
            };
        }

        public async Task<ResponseDetail> GetByUserAsync(long userId, int pageIndex, int pageSize)
        {
            IQueryable<VignettePurchase> query = _context.VignettePurchases
                .Where(v => v.UserId == userId)
                .OrderByDescending(v => v.CreatedAt);

            var list = pageSize == 0
                ? await query.ToListAsync()
                : await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();

            return new ResponseDetail
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Purchase list retrieved.",
                Data = list,
                DataLength = list.Count,
                IsError = false
            };
        }

        public async Task<ResponseDetail> UpdatePaymentAsync(VignettePaymentUpdateRequest model)
        {
            var purchase = await _context.VignettePurchases.FindAsync(model.PurchaseId);

            if (purchase == null)
            {
                return new ResponseDetail
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Message = "Purchase not found.",
                    IsError = true
                };
            }

            purchase.PaymentStatus = model.PaymentStatus;
            purchase.PaymentTransactionId = model.PaymentTransactionId;
            purchase.ReceiptPdfUrl = model.ReceiptPdfUrl;
            purchase.PaymentTimestamp = DateTime.UtcNow;
            purchase.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new ResponseDetail
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Payment updated successfully.",
                Data = purchase,
                DataLength = 1,
                IsError = false
            };
        }
    }

}
