using AutoMapper;
using Microsoft.Extensions.Configuration;
using Store.Data.Entities;
using Store.Data.Entities.OrderEntities;
using Store.Repository.Interfaces;
using Store.Repository.Specifications.OrderSpecs;
using Store.Service.Services.BasketService;
using Store.Service.Services.BasketService.Dtos;
using Store.Service.Services.OrderServices.Dtos;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.PaymentServices
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketService _basketService;
        private readonly IMapper _mapper;

        public PaymentService(IConfiguration configuration , IUnitOfWork unitOfWork , IBasketService basketService , IMapper mapper)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _basketService = basketService;
            _mapper = mapper;
        }
        public async Task<CustomerBasketDto> CreateOrUpdatePaymentIntent(CustomerBasketDto input)
        {

            StripeConfiguration.ApiKey = _configuration["Stripe:Secretkey"];

            if (input is null)
                throw new Exception("Basket is Empty");

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod, int>().GetByIdAsync(input.DeliveryMethodId.Value);

            if (deliveryMethod is null)
                throw new Exception("Delivery Method Not Provided");

            decimal shippingPrice = deliveryMethod.Price;

            foreach(var item in input.BasketItems)
            {
                var product = await _unitOfWork.Repository<Data.Entities.Product, int>().GetByIdAsync(item.ProductId);

                if (item.Price != product.Price)
                    item.Price = product.Price;


            }

            var service = new PaymentIntentService();

            PaymentIntent paymentIntent;

            if(string.IsNullOrEmpty(input.PaymentIntentId))
            {

                var options = new PaymentIntentCreateOptions
                {

                    Amount = (long)input.BasketItems.Sum(item => item.Quantity * (item.Price * 100)) + (long)(shippingPrice * 100),
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" }

                };

                paymentIntent = await service.CreateAsync(options);

                input.PaymentIntentId = paymentIntent.Id;
                input.ClientSecret = paymentIntent.ClientSecret;

            }
            else
            {
                var options = new PaymentIntentUpdateOptions
                {
                    Amount = (long)input.BasketItems.Sum(item => item.Quantity * (item.Price * 100)) + (long)(shippingPrice * 100),

                };

                await service.UpdateAsync(input.PaymentIntentId, options);

                
            }


            await _basketService.UpdateBasketAsync(input);

            return input;

        }

        public async Task<OrderDetailsDto> UpdateOrderPaymentFailed(string paymentIntentId)
        {

            var specs = new OrderWithPaymentIntentSpecification(paymentIntentId);

            var order = await _unitOfWork.Repository<Order, Guid>().GetWithSpecificationByIdAsync(specs);

            if (order is null)
                throw new Exception("Order Does Not Exist");

            order.OrderPaymentStatus = OrderPaymentStatus.Failed;

            _unitOfWork.Repository<Order, Guid>().Update(order);

            await _unitOfWork.CompleteAsync();

            var mappedOrder = _mapper.Map<OrderDetailsDto>(order);

            return mappedOrder;

            

        }

        public async Task<OrderDetailsDto> UpdateOrderPaymentSucceeded(string paymentIntentId)
        {

            var specs = new OrderWithPaymentIntentSpecification(paymentIntentId);

            var order = await _unitOfWork.Repository<Order, Guid>().GetWithSpecificationByIdAsync(specs);

            if (order is null)
                throw new Exception("Order Does Not Exist");

            order.OrderPaymentStatus = OrderPaymentStatus.Received;

            _unitOfWork.Repository<Order, Guid>().Update(order);

            await _unitOfWork.CompleteAsync();

            await _basketService.DeleteBasketAsync(order.BasketId);

            var mappedOrder = _mapper.Map<OrderDetailsDto>(order);

            return mappedOrder;




        }
    }
}
