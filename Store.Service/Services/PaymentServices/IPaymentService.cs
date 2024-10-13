using Store.Service.Services.BasketService.Dtos;
using Store.Service.Services.OrderServices.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.PaymentServices
{
    public interface IPaymentService
    {

        Task<CustomerBasketDto> CreateOrUpdatePaymentIntent(CustomerBasketDto input);

        Task<OrderDetailsDto> UpdateOrderPaymentSucceeded(string paymentIntentId);

        Task<OrderDetailsDto> UpdateOrderPaymentFailed(string paymentIntentId);




    }
}
