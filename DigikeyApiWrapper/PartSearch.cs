using ApiClient.Models;
using ApiClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigikeyApiWrapper
{
    /// <summary>
    /// Search for a specific part's specifications
    /// </summary>
    public class PartSearch
    {
        /// <summary>
        /// Service to access API
        /// </summary>
        private ApiClientService service;

        public PartSearch()
        {
            UpdateService();
        }

        /// <summary>
        /// Update the service with the new settings
        /// </summary>
        public void UpdateService()
        {
            service = new ApiClientService(ApiClientSettings.GetInstance());
        }

        /// <summary>
        /// Get the essentials product details
        /// </summary>
        /// <param name="MPN">Manufacturer Product Number of the part</param>
        public async Task<string> ProductDetails_Essentials(string MPN)
        {
            // MPN, SPN, description, category, pricing. Maybe add obsolete, productStatus, PrimaryPhoto, QuantityAvailable, MinimumOrderQuantity
            // Category seems to give nothing ?
            string response = await service.ProductDetails(MPN, "ManufacturerPartNumber,DigiKeyPartNumber,Manufacturer,DetailedDescription,StandardPricing[1]");

            return response;
        }

        /// <summary>
        /// Deserialize the received json
        /// </summary>
        public static DigikeyPart DeserializeProductDetails(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            
            return System.Text.Json.JsonSerializer.Deserialize<DigikeyPart>(json);
        }
    }
}
