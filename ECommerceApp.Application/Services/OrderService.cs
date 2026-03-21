using AutoMapper;
using ECommerceApp.Application.DTOs.Orders;
using ECommerceApp.Application.DTOs.QueryParams;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Enums;
using ECommerceApp.Domain.Interfaces;
using ECommerceApp.Domain.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBasketRepository _basketRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository, IBasketRepository basketRepository, IProductRepository productRepository, IUnitOfWork unitOfWork, ILogger<OrderService> logger, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _basketRepository = basketRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PagedResult<OrderListDto>> GetMyOrdersAsync(string userId, OrderQueryParams queryParams, CancellationToken cancellationToken)
        {
            var totalCount = await _orderRepository.CountAsync(userId, cancellationToken);

            if (totalCount == 0)
            {
                return new PagedResult<OrderListDto>
                {
                    Items = Array.Empty<OrderListDto>(),
                    TotalCount = 0
                };
            }

            var orders = await _orderRepository.GetMyOrdersAsync(userId, queryParams.PageNumber, queryParams.PageSize, cancellationToken);

            var items = _mapper.Map<List<OrderListDto>>(orders);

            return new PagedResult<OrderListDto>
            {
                Items = items,
                TotalCount = totalCount
            };
        }

        // TODO: Implement basket merge after login (merge cookie-based basket with user basket)
        public async Task<CreateOrderResponseDto> CreateOrderAsync(string userId, string basketId, CreateOrderRequestDto request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("CreateOrder started. UserId={UserId}, BasketId={BasketId}", userId, basketId);

            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("CreateOrder failed: user is not authenticated.");
                throw new UnauthorizedAccessException("User is not authenticated");
            }

            if (string.IsNullOrWhiteSpace(basketId))
            {
                _logger.LogWarning("CreateOrder failed: basketId is missing. UserId={UserId}", userId);
                throw new InvalidOperationException("Basket was not found.");
            }

            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket is null || basket.Items.Count == 0)
            {
                _logger.LogWarning("CreateOrder failed: basket is empty. UserId={UserId}, BasketId={BasketId}", userId, basketId);
                throw new InvalidOperationException("Basket is empty.");
            }

            var productIds = basket.Items.Select(x => x.ProductId).Distinct().ToList();

            var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

            var productDictionary = products.ToDictionary(x => x.Id);

            foreach (var item in basket.Items)
            {
                if (!productDictionary.TryGetValue(item.ProductId, out var product))
                    throw new InvalidOperationException($"Product with id {item.ProductId} was not found.");

                if (product.Stock < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for product '{product.Name}'.");
            }

            var order = new Order
            {
                UserId = userId,
                Status = OrderStatus.Pending
            };

            decimal totalAmount = 0m;

            foreach(var item in basket.Items)
            {
                var product = productDictionary[item.ProductId];

                var unitPrice = product.Price;
                var lineTotal = unitPrice * item.Quantity;

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = unitPrice,
                    Quantity = item.Quantity,
                    LineTotal = lineTotal,
                };

                order.Items.Add(orderItem);

                totalAmount += lineTotal;
            }

            order.TotalAmount = totalAmount;


            // TODO: Ensure consistency between order persistence (SQL) and basket deletion (Redis).
            // Consider retry, eventual consistency (background service) or event-driven approach

            await _orderRepository.AddAsync(order, cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Order persisted successfully. OrderId={OrderId}, UserId={UserId}, TotalAmount={TotalAmount}", order.Id, userId, totalAmount);

            var basketDeleted = await _basketRepository.DeleteBasketAsync(basketId);
            if (!basketDeleted)
            {
                //Retry: 
                //_logger.LogWarning("Basket delete failed. Retrying...");

                //basketDeleted = await _basketRepository.DeleteBasketAsync(basketId);
                _logger.LogError("Basket deletion failed after order creation. BasketId={BasketId}, OrderId={OrderId}", basketId,order.Id);

                //throw new InvalidOperationException("Basket could not be cleared after checkout.");
            }

            _logger.LogInformation("Basket cleared after checkout. BasketId={BasketId}, OrderId={OrderId}", basketId, order.Id );

            return new CreateOrderResponseDto(
                order.Id,
                order.TotalAmount,
                order.Status.ToString(),
                order.Items.Count
            );
        }
    }
}
