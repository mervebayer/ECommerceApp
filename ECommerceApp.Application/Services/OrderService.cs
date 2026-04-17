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
using System.Net;
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
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly IValidator<CreateOrderRequestDto> _createOrderValidator;
        private readonly IValidator<PayOrderRequestDto> _payOrderValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly ICheckoutSettings _checkoutSettings;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IUserRepository _userRepository;
        private readonly IPaymentTransactionRepository _paymentTransactionRepository;
        private readonly INotificationRealtimeService _notificationRealtimeService;
        private readonly IEmailService _emailService;

        public OrderService(IOrderRepository orderRepository, IBasketRepository basketRepository, IProductRepository productRepository, IUnitOfWork unitOfWork, ILogger<OrderService> logger, IMapper mapper, IOrderNumberGenerator orderNumberGenerator, IUserAddressRepository userAddressRepository, IValidator<CreateOrderRequestDto> createOrderValidator, IValidator<PayOrderRequestDto> payOrderValidator, ICheckoutSettings checkoutSettings, IPaymentGateway paymentGateway, IUserRepository userRepository, IPaymentTransactionRepository paymentTransactionRepository, INotificationRepository notificationRepository, INotificationRealtimeService notificationRealtimeService, IEmailService emailService)
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
            _payOrderValidator = payOrderValidator;
            _checkoutSettings = checkoutSettings;
            _paymentGateway = paymentGateway;
            _userRepository = userRepository;
            _paymentTransactionRepository = paymentTransactionRepository;
            _notificationRepository = notificationRepository;
            _notificationRealtimeService = notificationRealtimeService;
            _emailService = emailService;
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

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

            await TrySendOrderEmailAsync(
                user?.Email,
                "Siparisiniz iptal edildi",
                            $"""
                <h2>Merhaba {user?.FirstName ?? "Sayin Musteri"},</h2>
                <p><strong>{order.OrderNumber}</strong> numarali siparisiniz iptal edildi.</p>
                """,
                order.Id);

            _logger.LogInformation("Order cancelled by user. OrderId={OrderId}, UserId={UserId}", orderId, userId);
        }

        public async Task CancelOrderByAdminAsync(long orderId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var order = await _orderRepository.GetOrderByIdWithItemsAsync(orderId, cancellationToken)
                ?? throw new NotFoundException("Order not found.");

            await CancelOrderInternalAsync(order, cancellationToken);

            var notification = new Notification
            {
                Title = "Siparişiniz iptal edildi",
                Message = $"{order.OrderNumber} numaralı siparişiniz iptal edildi.",
                Type = NotificationType.OrderCancelled,
                Audience = NotificationAudience.Customer,
                ReceiverUserId = order.UserId,
                OrderId = order.Id,
                Link = $"/orders/{order.Id}"
            };

            await _notificationRepository.AddAsync(notification, cancellationToken);
            await CreateStaffNotificationsAsync(
                "Sipariş iptal edildi",
                $"{order.OrderNumber} numaralı sipariş yönetici tarafından iptal edildi.",
                NotificationType.OrderCancelled,
                order.Id,
                $"/admin/orders/{order.Id}",
                cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            await TryPublishRealtimeAsync(
                    () => _notificationRealtimeService.NotifyOrderUpdatedAsync(
                        "Sipariş iptal edildi",
                        $"{order.OrderNumber} numaralı sipariş yönetici tarafından iptal edildi.",
                        order.UserId,
                        "Siparişiniz iptal edildi",
                        $"{order.OrderNumber} numaralı siparişiniz iptal edildi.",
                        order.Id,
                        cancellationToken),
                    "OrderCancelledByAdmin",
                    order.Id);

            var orderOwner = await _userRepository.GetByIdAsync(order.UserId, cancellationToken);

            await TrySendOrderEmailAsync(
                orderOwner?.Email,
                "Siparisiniz iptal edildi",
                $"""
                <h2>Merhaba {orderOwner?.FirstName ?? "Sayin Musteri"},</h2>
                <p><strong>{order.OrderNumber}</strong> numarali siparisiniz yonetim tarafindan iptal edildi.</p>
                """,
                order.Id);


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
                await ApplyConfirmedStatusAsync(order, cancellationToken);
            }
            else
            {
                if (newStatus == OrderStatus.Expired)
                {
                    order.ReservationExpiresAt = null;
                }

                order.Status = newStatus;
            }

            _orderRepository.Update(order);

            var notification = new Notification
            {
                Title = "Sipariş durumu güncellendi",
                Message = $"{order.OrderNumber} numaralı siparişinizin durumu {newStatus} olarak güncellendi.",
                Type = NotificationType.OrderStatusChanged,
                Audience = NotificationAudience.Customer,
                ReceiverUserId = order.UserId,
                OrderId = order.Id,
                Link = $"/orders/{order.Id}"
            };

            await _notificationRepository.AddAsync(notification, cancellationToken);
            await CreateStaffNotificationsAsync(
                "Sipariş durumu güncellendi",
                $"{order.OrderNumber} numaralı siparişin durumu {newStatus} olarak güncellendi.",
                NotificationType.OrderStatusChanged,
                order.Id,
                $"/admin/orders/{order.Id}",
                cancellationToken);

            await _unitOfWork.CommitAsync(cancellationToken);

            var orderOwner = await _userRepository.GetByIdAsync(order.UserId, cancellationToken);

            if (newStatus == OrderStatus.Shipped)
            {
                await TrySendOrderEmailAsync(
                    orderOwner?.Email,
                    "Siparisiniz kargoya verildi",
                    $"""
                            <h2>Merhaba {orderOwner?.FirstName ?? "Sayin Musteri"},</h2>
                            <p><strong>{order.OrderNumber}</strong> numarali siparisiniz kargoya verildi.</p>
                            <p>Siparisinizi siparis detay ekranindan takip edebilirsiniz.</p>
                            """,
                                        order.Id);
                                }
           else if (newStatus == OrderStatus.Delivered)
           {
                await TrySendOrderEmailAsync(
                     orderOwner?.Email,
                     "Siparisiniz teslim edildi",
                     $"""
                            <h2>Merhaba {orderOwner?.FirstName ?? "Sayin Musteri"},</h2>
                            <p><strong>{order.OrderNumber}</strong> numarali siparisiniz teslim edildi.</p>
                            <p>Bizi tercih ettiginiz icin tesekkur ederiz.</p>
                            """,
                                        order.Id);
                                }

            await TryPublishRealtimeAsync(
                    () => _notificationRealtimeService.NotifyOrderUpdatedAsync(
                        "Sipariş durumu güncellendi",
                        $"{order.OrderNumber} numaralı siparişin durumu {newStatus} olarak güncellendi.",
                        order.UserId,
                        "Sipariş durumu güncellendi",
                        $"{order.OrderNumber} numaralı siparişinizin durumu {newStatus} olarak güncellendi.",
                        order.Id,
                        cancellationToken),
                    "OrderStatusUpdated",
                    order.Id);

            _logger.LogInformation("Order status updated successfully. OrderId={OrderId}, UserId={UserId}, NewStatus={NewStatus}", orderId, newStatus);
        }

        public async Task<CreateOrderResponseDto> CreateOrderAsync(string userId, string basketId, CreateOrderRequestDto request, CancellationToken cancellationToken)
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
                ReservationExpiresAt = DateTime.UtcNow.AddMinutes(_checkoutSettings.ReservationTimeoutMinutes),
                BasketId = basketId
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

            // TODO: Ensure consistency between order persistence (SQL) and basket deletion (Redis).
            // Consider retry, eventual consistency (background service) or event-driven approach

            await _orderRepository.AddAsync(order, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            return new CreateOrderResponseDto(
                order.Id,
                order.OrderNumber,
                order.TotalAmount,
                order.Status.ToString(),
                order.Items.Count
            );

        }

        //TODO: change to 3DS
        public async Task<PayOrderResponseDto> PayOrderAsync(string userId, long orderId, string buyerIp, PayOrderRequestDto request, CancellationToken cancellationToken)
        {
            var validationResult = await _payOrderValidator.ValidateAsync(request, cancellationToken);
            validationResult.ThrowIfInvalid();

            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
            if (user is null)
                throw new NotFoundException("User not found.");

            var order = await _orderRepository.GetByIdAndUserIdAsync(userId, orderId, cancellationToken);
            if (order is null)
                throw new NotFoundException("Order not found.");

            if (order.Status != OrderStatus.PendingPayment)
                throw new BusinessRuleException("Only pending payment orders can be paid.");

            var activeThreshold = DateTime.UtcNow.AddMinutes(-5);

            var expiredPendingTransactions = await _paymentTransactionRepository.GetExpiredPendingTransactionsAsync(order.Id, activeThreshold, cancellationToken);

            if (expiredPendingTransactions.Any())
            {
                foreach (var expiredTransaction in expiredPendingTransactions)
                {
                    expiredTransaction.Status = PaymentTransactionStatus.Expired;
                    _paymentTransactionRepository.Update(expiredTransaction);
                }

                await _unitOfWork.CommitAsync(cancellationToken);
            }

            var idempotentResponse = await GetIdempotentPaymentResponseAsync(order, request.IdempotencyKey, cancellationToken);

            if (idempotentResponse is not null)
                return idempotentResponse;


            var hasSuccessfulTransaction = await _paymentTransactionRepository.HasSuccessfulTransactionAsync(order.Id, cancellationToken);
            if (hasSuccessfulTransaction)
                throw new BusinessRuleException("This order has already been paid.");

            var hasPendingTransaction = await _paymentTransactionRepository.HasPendingTransactionAsync(order.Id, activeThreshold, cancellationToken);
            if (hasPendingTransaction)
                throw new BusinessRuleException("A payment transaction is already in progress for this order.");

            if (order.ReservationExpiresAt.HasValue && order.ReservationExpiresAt.Value <= DateTime.UtcNow)
            {
                order.Status = OrderStatus.Expired;
                order.ReservationExpiresAt = null;
                _orderRepository.Update(order);
                await _unitOfWork.CommitAsync(cancellationToken);

                throw new BusinessRuleException("This order payment window has expired.");
            }

            var transaction = new PaymentTransaction
            {
                OrderId = order.Id,
                Status = PaymentTransactionStatus.Pending,
                ConversationId = order.OrderNumber,
                IdempotencyKey = request.IdempotencyKey
            };

            await _paymentTransactionRepository.AddAsync(transaction, cancellationToken);
            await _unitOfWork.CommitAsync(cancellationToken);

            var paymentRequest = new ProcessPaymentRequest
            {
                ConversationId = order.OrderNumber,
                BasketId = order.BasketId ?? order.OrderNumber,
                Price = order.TotalAmount,
                PaidPrice = order.TotalAmount,
                PaymentCard = request.PaymentCard,

                BuyerId = user.Id,
                BuyerIp = buyerIp,
                BuyerEmail = user.Email ?? string.Empty,
                BuyerFirstName = user.FirstName ?? string.Empty,
                BuyerLastName = user.LastName ?? string.Empty,
                BuyerPhoneNumber = user.PhoneNumber ?? order.ShippingPhoneNumber ?? string.Empty,

                //TODO: update to real tckn
                BuyerIdentityNumber = "11111111111",

                ShippingContactName = order.ShippingContactName,
                AddressLine = order.ShippingAddressLine,
                City = order.ShippingCity,
                Country = order.ShippingCountry,
                ZipCode = order.ShippingPostalCode ?? string.Empty,

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
            PaymentResult paymentResult;

            try
            {

                paymentResult = await _paymentGateway.ProcessAsync(paymentRequest, cancellationToken);
            }
            catch (Exception ex)
            {

                transaction.Status = PaymentTransactionStatus.Failed;
                transaction.ErrorMessage = ex.Message;
                _paymentTransactionRepository.Update(transaction);
                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogError(ex, "Payment processing failed with exception. OrderId={OrderId}", order.Id);
                throw;
            }

            if (paymentResult.IsSuccess)
            {
                transaction.Status = PaymentTransactionStatus.Succeeded;
                transaction.ProviderPaymentId = paymentResult.PaymentId;
                transaction.ErrorCode = null;
                transaction.ErrorMessage = null;
                _paymentTransactionRepository.Update(transaction);
                await _unitOfWork.CommitAsync(cancellationToken);

                try
                {
                    await ApplyConfirmedStatusAsync(order, cancellationToken);
                    _orderRepository.Update(order);
                    await _unitOfWork.CommitAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    transaction.ErrorMessage = $"Payment succeeded but order confirmation failed: {ex.Message}";
                    _paymentTransactionRepository.Update(transaction);
                    await _unitOfWork.CommitAsync(cancellationToken);

                    _logger.LogError(ex,"Payment succeeded but order confirmation failed. OrderId={OrderId}, PaymentTransactionId={PaymentTransactionId}", order.Id, transaction.Id);
                    throw;
                }

                var basketId = order.BasketId;
                if (basketId is not null)
                {
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

                var userNotification = new Notification
                {
                    Title = "Sipariş oluştu",
                    Message = $"{order.OrderNumber} numaralı siparişiniz başarıyla oluşturuldu.",
                    Type = NotificationType.PaymentReceived,
                    Audience = NotificationAudience.Customer,
                    ReceiverUserId = order.UserId,
                    OrderId = order.Id,
                    Link = $"/orders/{order.Id}"
                };

                await _notificationRepository.AddAsync(userNotification, cancellationToken);

                await CreateStaffNotificationsAsync(
                    "Yeni sipariş geldi",
                    $"{order.OrderNumber} numaralı siparişin ödemesi tamamlandı.",
                    NotificationType.OrderCreated,
                    order.Id,
                    $"/admin/orders/{order.Id}",
                    cancellationToken);

                await _unitOfWork.CommitAsync(cancellationToken);

                await TryPublishRealtimeAsync(
                    () => _notificationRealtimeService.NotifyBackofficeAsync(
                        "Yeni sipariş geldi",
                        $"{order.OrderNumber} numaralı siparişin ödemesi tamamlandı.",
                        order.Id,
                        cancellationToken),
                    "PaymentCompletedBackoffice",
                    order.Id);

                await TryPublishRealtimeAsync(
                    () => _notificationRealtimeService.NotifyUserAsync(
                        order.UserId,
                        "Sipariş oluştu",
                        $"{order.OrderNumber} numaralı siparişiniz başarıyla oluşturuldu.",
                        order.Id,
                        cancellationToken),
                    "PaymentCompletedUser",
                    order.Id);
                
                await TrySendOrderEmailAsync(
                        user.Email,
                        "Siparisiniz onaylandi",
                        $"""
                        <h2>Merhaba {user.FirstName} {user.LastName},</h2>
                        <p><strong>{order.OrderNumber}</strong> numarali siparisinizin odemesi basariyla alindi.</p>
                        <p>Siparisiniz onaylandi ve hazirlama sureci baslayacak.</p>
                        <p>Toplam tutar: {order.TotalAmount:N2} TL</p>
                        """,
                        order.Id);

            }
            else
            {

                transaction.Status = PaymentTransactionStatus.Failed;
                transaction.ProviderPaymentId = paymentResult.PaymentId;
                transaction.ErrorCode = paymentResult.ErrorCode;
                transaction.ErrorMessage = paymentResult.ErrorMessage;
                _paymentTransactionRepository.Update(transaction);

                await _unitOfWork.CommitAsync(cancellationToken);
            }

            return new PayOrderResponseDto(orderId, order.OrderNumber, order.Status.ToString(), paymentResult.IsSuccess, paymentResult.IsSuccess ? null : paymentResult.ErrorMessage);
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

        private async Task ApplyConfirmedStatusAsync(Order order, CancellationToken cancellationToken)
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
            order.Status = OrderStatus.Confirmed;
        }

        private async Task<PayOrderResponseDto?> GetIdempotentPaymentResponseAsync(Order order, string idempotencyKey, CancellationToken cancellationToken)
        {
            var existingTransaction = await _paymentTransactionRepository.GetByOrderIdAndIdempotencyKeyAsync(order.Id, idempotencyKey, cancellationToken);

            if (existingTransaction is null)
                return null;

            return existingTransaction.Status switch
            {
                PaymentTransactionStatus.Succeeded => new PayOrderResponseDto(
                    order.Id,
                    order.OrderNumber,
                    order.Status.ToString(),
                    true,
                    null),

                PaymentTransactionStatus.Failed => new PayOrderResponseDto(
                    order.Id,
                    order.OrderNumber,
                    order.Status.ToString(),
                    false,
                    existingTransaction.ErrorMessage),

                PaymentTransactionStatus.Pending => throw new BusinessRuleException(
                    "This payment request is already being processed."),

                PaymentTransactionStatus.Expired => throw new BusinessRuleException(
                    "This payment request has expired. Please retry with a new idempotency key."),

                _ => throw new BusinessRuleException("Payment transaction state is invalid.")
            };
        }

        private async Task CreateStaffNotificationsAsync(string title, string message, NotificationType type, long orderId, string link, CancellationToken cancellationToken)
        {
            var staffUsers = await _userRepository.GetUsersInRolesAsync(new[] { "Admin", "StoreManager" }, cancellationToken);

            foreach (var staffUser in staffUsers)
            {
                if (string.IsNullOrWhiteSpace(staffUser.Id))
                    continue;

                await _notificationRepository.AddAsync(new Notification
                {
                    Title = title,
                    Message = message,
                    Type = type,
                    Audience = NotificationAudience.Backoffice,
                    ReceiverUserId = staffUser.Id,
                    OrderId = orderId,
                    Link = link
                }, cancellationToken);
            }
        }

        private async Task TryPublishRealtimeAsync(Func<Task> publishAction, string operationName, long orderId)
        {
            try
            {
                await publishAction();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Realtime notification publish failed. Operation={Operation}, OrderId={OrderId}", operationName, orderId);
            }
        }

        private async Task TrySendOrderEmailAsync(string? toEmail, string subject, string htmlBody, long orderId)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                return;

            try
            {
                await _emailService.SendAsync(toEmail, subject, htmlBody);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Order email could not be sent. OrderId={OrderId}, To={ToEmail}", orderId, toEmail);
            }
        }

    }
}
