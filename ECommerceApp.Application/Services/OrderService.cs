using AutoMapper;
using ECommerceApp.Application.DTOs.Orders;
using ECommerceApp.Application.DTOs.Orders.Admin;
using ECommerceApp.Application.DTOs.Payments;
using ECommerceApp.Application.DTOs.QueryParams;
using ECommerceApp.Application.Extensions;
using ECommerceApp.Application.Interfaces;
using ECommerceApp.Domain.Entities;
using ECommerceApp.Domain.Enums;
using ECommerceApp.Domain.Exceptions;
using ECommerceApp.Domain.Interfaces;
using ECommerceApp.Domain.Interfaces.Repositories;
using FluentValidation;
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
        private readonly IOrderNumberGenerator _orderNumberGenerator;
        private readonly IOrderRepository _orderRepository;
        private readonly IBasketRepository _basketRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly IValidator<CreateOrderRequestDto> _createOrderValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly ICheckoutSettings _checkoutSettings;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IUserRepository _userRepository;

        public OrderService(IOrderRepository orderRepository, IBasketRepository basketRepository, IProductRepository productRepository, IUnitOfWork unitOfWork, ILogger<OrderService> logger, IMapper mapper, IOrderNumberGenerator orderNumberGenerator, IUserAddressRepository userAddressRepository, IValidator<CreateOrderRequestDto> createOrderValidator, ICheckoutSettings checkoutSettings, IPaymentGateway paymentGateway, IUserRepository userRepository)
        {
            _orderRepository = orderRepository;
            _basketRepository = basketRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _orderNumberGenerator = orderNumberGenerator;
            _userAddressRepository = userAddressRepository;
            _createOrderValidator = createOrderValidator;
            _checkoutSettings = checkoutSettings;
            _paymentGateway = paymentGateway;
            _userRepository = userRepository;
        }

        public async Task<PagedResult<OrderListDto>> GetMyOrdersAsync(string userId, OrderQueryParams queryParams, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

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

        public async Task<PagedResult<AdminOrderListDto>> GetAllOrdersAsync(AdminOrderQueryParams queryParams, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var statusFilter = queryParams.Status.HasValue && Enum.IsDefined(typeof(OrderStatus), queryParams.Status.Value)
                ? queryParams.Status
                : null;

            var totalCount = await _orderRepository.CountAllAsync(statusFilter, queryParams.OrderNumber, queryParams.UserId, queryParams.StartDate, queryParams.EndDate, cancellationToken);

            if (totalCount == 0)
            {
                return new PagedResult<AdminOrderListDto>
                {
                    Items = Array.Empty<AdminOrderListDto>(),
                    TotalCount = 0
                };
            }

            var orders = await _orderRepository.GetAllOrdersAsync(queryParams.PageNumber, queryParams.PageSize, statusFilter, queryParams.OrderNumber, queryParams.UserId, queryParams.StartDate, queryParams.EndDate, cancellationToken);

            var items = _mapper.Map<List<AdminOrderListDto>>(orders);

            return new PagedResult<AdminOrderListDto>
            {
                Items = items,
                TotalCount = totalCount
            };
        }


        public async Task<AdminOrderDetailDto> GetOrderById(long orderId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var order = await _orderRepository.GetOrderByIdAsync(orderId, cancellationToken)
                    ?? throw new NotFoundException("Order not found.");

            return _mapper.Map<AdminOrderDetailDto>(order);
        }

        public async Task<OrderDetailDto> GetOrderByIdAndUserIdAsync(string userId, long orderId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var order = await _orderRepository.GetByIdAndUserIdAsync(userId, orderId, cancellationToken)
                    ?? throw new NotFoundException("Order not found.");

            return _mapper.Map<OrderDetailDto>(order);

        }

        public async Task CancelOrderAsync(string userId, long orderId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var order = await _orderRepository.GetByIdAndUserIdAsync(userId, orderId, cancellationToken)
                ?? throw new NotFoundException("Order not found.");

            await CancelOrderInternalAsync(order, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Order cancelled by user. OrderId={OrderId}, UserId={UserId}", orderId, userId);
        }

        public async Task CancelOrderByAdminAsync(long orderId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var order = await _orderRepository.GetOrderByIdWithItemsAsync(orderId, cancellationToken)
                ?? throw new NotFoundException("Order not found.");

            await CancelOrderInternalAsync(order, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Order cancelled by admin. OrderId={OrderId}", orderId);
        }

        public async Task UpdateOrderStatusAsync(long orderId, OrderStatus newStatus, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var order = await _orderRepository.GetOrderByIdWithItemsAsync(orderId, cancellationToken);

            if (order is null)
                throw new NotFoundException("Order not found.");

            if (!IsValidStatusTransition(order.Status, newStatus))          
                throw new BusinessRuleException($"Invalid status transition from {order.Status} to {newStatus}.");
            
            //stock update
            if (order.Status == OrderStatus.PendingPayment && newStatus == OrderStatus.Confirmed)
            {
                var productIds = order.Items.Select(x => x.ProductId).Distinct().ToList();

                var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
                var productDictionary = products.ToDictionary(x => x.Id);

                var reservedQuantities = await _orderRepository.GetReservedQuantitiesExcludingOrderAsync(productIds, order.Id, cancellationToken);

                foreach (var item in order.Items)
                {
                    if (!productDictionary.TryGetValue(item.ProductId, out var product))
                        throw new BusinessRuleException($"Product with id {item.ProductId} was not found.");

                    var reservedByOthers = reservedQuantities.GetValueOrDefault(item.ProductId, 0);
                    var availableStock = product.Stock - reservedByOthers;

                    if (availableStock < item.Quantity)                    
                        throw new BusinessRuleException($"Insufficient available stock for product '{product.Name}'. Available: {availableStock}, Required: {item.Quantity}.");                   
                }

                foreach (var item in order.Items)
                {
                    var product = productDictionary[item.ProductId];
                    product.Stock -= item.Quantity;
                }

                order.ReservationExpiresAt = null;
            }

            if (newStatus == OrderStatus.Expired)
            {
                order.ReservationExpiresAt = null;
            }

            order.Status = newStatus;

            _orderRepository.Update(order);
            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation("Order status updated successfully. OrderId={OrderId}, UserId={UserId}, NewStatus={NewStatus}", orderId, newStatus);
        }

        // TODO: Implement basket merge after login (merge cookie-based basket with user basket)

        public async Task<CreateOrderResponseDto> CreateOrderAsync(string userId, string basketId, string buyerIp, CreateOrderRequestDto request, CancellationToken cancellationToken)
        {
            var validationResult = await _createOrderValidator.ValidateAsync(request, cancellationToken);
            validationResult.ThrowIfInvalid();

            // TODO: Move pending payment expiration cleanup to a background service instead of triggering it during checkout.
            await ExpirePendingPaymentOrdersAsync(cancellationToken);

            if (string.IsNullOrWhiteSpace(userId))           
                throw new UnauthorizedException("User is not authenticated");
            

            if (string.IsNullOrWhiteSpace(basketId))
                throw new BusinessRuleException("Basket was not found.");
            

            var basket = await _basketRepository.GetBasketAsync(basketId);

            if (basket is null || basket.Items.Count == 0)            
                throw new BusinessRuleException("Basket is empty.");
           
            var address = await _userAddressRepository.GetByIdAndUserIdAsync(request.UserAddressId, userId, cancellationToken);
            if (address is null)           
                throw new BusinessRuleException("Address not found.");

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is null)
                throw new NotFoundException("User not found.");

            var productIds = basket.Items.Select(x => x.ProductId).Distinct().ToList();

            var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
            var productDictionary = products.ToDictionary(x => x.Id);

            var reservedQuantities = await _orderRepository.GetReservedQuantitiesAsync(productIds, cancellationToken);

            foreach (var item in basket.Items)
            {
                if (!productDictionary.TryGetValue(item.ProductId, out var product))
                    throw new BusinessRuleException($"Product with id {item.ProductId} was not found.");

                var reservedQuantity = reservedQuantities.GetValueOrDefault(item.ProductId, 0);
                var availableStock = product.Stock - reservedQuantity;

                if (availableStock < item.Quantity)
                {
                    throw new BusinessRuleException($"Insufficient available stock for product '{product.Name}'. Available: {availableStock}, Requested: {item.Quantity}.");
                }
            }

            var order = new Order
            {
                OrderNumber = _orderNumberGenerator.Generate(),
                UserId = userId,
                Status = OrderStatus.PendingPayment,
                ReservationExpiresAt = DateTime.UtcNow.AddMinutes(_checkoutSettings.ReservationTimeoutMinutes)
            };

            ApplyShippingAddress(order, address);

            decimal totalAmount = 0m;

            foreach (var item in basket.Items)
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


            var paymentRequest = new ProcessPaymentRequest
            {
                ConversationId = order.OrderNumber,
                BasketId = basketId,
                Price = order.TotalAmount,
                PaidPrice = order.TotalAmount,
                PaymentCard = request.PaymentCard,

                BuyerId = user.Id,
                BuyerIp = buyerIp,
                BuyerEmail = user.Email ?? string.Empty,
                BuyerFirstName = user.FirstName ?? string.Empty,
                BuyerLastName = user.LastName ?? string.Empty,
                BuyerPhoneNumber = user.PhoneNumber ?? address.PhoneNumber ?? string.Empty,

                //TODO: update to real tckn
                BuyerIdentityNumber = "11111111111",

                ShippingContactName = address.ContactName,
                AddressLine = address.AddressLine,
                City = address.City,
                Country = address.Country,
                ZipCode = address.PostalCode ?? string.Empty,

                Items = order.Items
                    .SelectMany(item => Enumerable.Range(0, item.Quantity).Select(_ => new PaymentBasketItemDto
                    {
                        Id = item.ProductId.ToString(),
                        Name = item.ProductName,
                        Category1 = "General",
                        ItemType = "PHYSICAL",
                        Price = item.UnitPrice
                    }))
                    .ToList()
            };
       


            // TODO: Ensure consistency between order persistence (SQL) and basket deletion (Redis).
            // Consider retry, eventual consistency (background service) or event-driven approach

            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            var paymentResult = await _paymentGateway.ProcessAsync(paymentRequest, cancellationToken);

            if (paymentResult.IsSuccess)
            {
                await UpdateOrderStatusAsync(order.Id, OrderStatus.Confirmed, cancellationToken);
                order.Status = OrderStatus.Confirmed;
                order.ReservationExpiresAt = null;

                var basketDeleted = await _basketRepository.DeleteBasketAsync(basketId);
                if (!basketDeleted)
                {
                    _logger.LogError("Basket deletion failed after successful payment. BasketId={BasketId}, OrderId={OrderId}", basketId, order.Id);
                }
                else
                {
                    _logger.LogInformation("Basket cleared after successful checkout. BasketId={BasketId}, OrderId={OrderId}", basketId, order.Id);
                }
            }

            return new CreateOrderResponseDto(
                order.OrderNumber,
                order.TotalAmount,
                order.Status.ToString(),
                order.Items.Count,
                    paymentResult.IsSuccess,
                    paymentResult.IsSuccess ? null : paymentResult.ErrorMessage
            );
        }
        private static bool IsValidStatusTransition(OrderStatus current, OrderStatus next)
        {
            switch (current)
            {
                case OrderStatus.PendingPayment:
                    return next == OrderStatus.Confirmed
                        || next == OrderStatus.Expired;

                case OrderStatus.Confirmed:
                    return next == OrderStatus.Preparing;

                case OrderStatus.Preparing:
                    return next == OrderStatus.Shipped;

                case OrderStatus.Shipped:
                    return next == OrderStatus.Delivered
                        || next == OrderStatus.DeliveryFailed;

                case OrderStatus.DeliveryFailed:
                    return next == OrderStatus.Shipped;

                case OrderStatus.Delivered:
                    return next == OrderStatus.Completed;

                case OrderStatus.Completed:
                case OrderStatus.Cancelled:
                case OrderStatus.Expired:
                    return false;

                default:
                    return false;
            }
        }
        private static void ApplyShippingAddress(Order order, UserAddress address)
        {
            order.ShippingTitle = address.Title;
            order.ShippingContactName = address.ContactName;
            order.ShippingPhoneNumber = address.PhoneNumber;
            order.ShippingCountry = address.Country;
            order.ShippingCity = address.City;
            order.ShippingDistrict = address.District;
            order.ShippingPostalCode = address.PostalCode;
            order.ShippingAddressLine = address.AddressLine;
        }
        private async Task ExpirePendingPaymentOrdersAsync(CancellationToken cancellationToken)
        {
            var expiredOrders = await _orderRepository.GetExpiredPendingPaymentOrdersAsync(cancellationToken);

            if (!expiredOrders.Any())
                return;

            foreach (var order in expiredOrders)
            {
                order.Status = OrderStatus.Expired;
                order.ReservationExpiresAt = null;
                _orderRepository.Update(order);
            }

            await _unitOfWork.CommitAsync(cancellationToken);

        }
        private async Task CancelOrderInternalAsync(Order order, CancellationToken cancellationToken)
        {
            if (order.Status != OrderStatus.PendingPayment && order.Status != OrderStatus.Confirmed)
                throw new BusinessRuleException("Only pending payment or confirmed orders can be cancelled.");

            if (order.Status == OrderStatus.Confirmed)
            {
                var productIds = order.Items.Select(x => x.ProductId).Distinct().ToList();
                var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);
                var productDictionary = products.ToDictionary(x => x.Id);

                foreach (var item in order.Items)
                {
                    if (!productDictionary.TryGetValue(item.ProductId, out var product))
                        throw new NotFoundException($"Product with id {item.ProductId} was not found.");

                    product.Stock += item.Quantity;
                }
            }

            order.Status = OrderStatus.Cancelled;
            order.ReservationExpiresAt = null;
        }


    }
}
