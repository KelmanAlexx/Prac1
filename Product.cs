using System;

namespace ExamWinFormsApp1
{
    public class Product
    {
        public string Article { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public decimal Price { get; set; }
        public string Supplier { get; set; }
        public string Manufacturer { get; set; }
        public string Category { get; set; }
        public decimal Discount { get; set; }
        public int StockQuantity { get; set; }
        public string Description { get; set; }
        public string Photo { get; set; }

        public Product()
        {
            Article = "";
            ProductName = "";
            Unit = "";
            Supplier = "";
            Manufacturer = "";
            Category = "";
            Description = "";
            Photo = "";
        }
    }
}