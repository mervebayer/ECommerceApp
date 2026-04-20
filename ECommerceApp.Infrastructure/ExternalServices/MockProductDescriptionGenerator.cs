using ECommerceApp.Application.DTOs.Products;
using ECommerceApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerceApp.Infrastructure.ExternalServices
{
    public class MockProductDescriptionGenerator : IProductDescriptionGenerator
    {
        public Task<string> GenerateAsync(ProductDescriptionGenerationInput input, CancellationToken cancellationToken = default)
        {
            var categoryText = string.IsNullOrWhiteSpace(input.CategoryName)
                ? "Bu urun"
                : $"{input.CategoryName} kategorisindeki bu urun";

            //var imageText = string.IsNullOrWhiteSpace(input.ImageUrl)
            //    ? string.Empty
            //    : " Urunun gorseli de dikkate alinarak aciklama hazirlandi.";

            var description =
                $"{input.Name}, şık görünümü ve rahat kesimiyle stilinize modern bir dokunuş katar. " +
                $"{categoryText}, kalite ve islevselligi bir araya getirir. ";
                //+
                //$"Fiyat avantaji sunan bu urun, ihtiyaca yonelik pratik bir tercih olarak one cikar.{imageText}";

            return Task.FromResult(description);
        }
    }
}
