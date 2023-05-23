namespace DigikeyApiWrapper
{
    /// <summary>
    /// Define an imported part. Used to deserialize a json string
    /// </summary>
    public class DigikeyPart
    {
        public DigikeyStandardPricing[] StandardPricing { get; set; }
        public float UnitPrice
        {
            get
            {
                if (StandardPricing != null && StandardPricing.Length > 0)
                {
                    return StandardPricing[0].UnitPrice / StandardPricing[0].BreakQuantity;
                }
                return 0;
            }
        }
        public string DetailedDescription { get; set; }
        public string ManufacturerPartNumber { get; set; }
        public string DigiKeyPartNumber { get; set; }
        public DigikeyManufacturer Manufacturer { get; set; }
        public string ManufacturerString { get => Manufacturer ?? string.Empty; }

        public class DigikeyStandardPricing
        {
            public int BreakQuantity { get; set; }
            public float UnitPrice { get; set; }
            public float TotalPrice { get; set; }
        }

        /// <summary>
        /// Implicit cast to string
        /// </summary>
        public class DigikeyManufacturer
        {
            public string Value { get; set; }

            public static implicit operator string(DigikeyManufacturer p)
            {
                return p.Value;
            }
        }
    }
}