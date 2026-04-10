namespace ECommerceApp.API.Services
{
    public interface IBasketIdentityService
    {
        Task<string> GetOrCreateBasketIdAsync(CancellationToken cancellationToken = default);
    }
}
