﻿namespace SimpleShop.Models
{
    public class Stock
    {
        public int StockId { get; set; }
        public int Quantity { get; set; }

        public Product Product { get; set; }
    }
}