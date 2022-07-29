﻿namespace WebAPI.Classes
{
    public class ProductQueryParameters : QueryParameters
    {
        public string? Sku { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Name { get; set; }
        public string? SearchTerm { get; set; }

    }
}
