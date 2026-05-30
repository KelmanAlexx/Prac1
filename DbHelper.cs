using ExamWinFormsApp1;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace WinFormsApp1
{
    public static class DbHelper
    {
        private static string ConnectionString = "Host=localhost;Database=shoes_store;Username=admin4ik;Password=1";

        public static async Task<DataTable> CheckUserAsync(string login, string password)
        {
            DataTable table = new DataTable();
            string sql = "SELECT full_name, role FROM users WHERE login = @login AND password = @password";

            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@login", login);
                    cmd.Parameters.AddWithValue("@password", password);

                    using (NpgsqlDataReader reader = (NpgsqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }

        public static async Task<List<Product>> GetProductsAsync(string search, string sortBy)
        {
            List<Product> list = new List<Product>();
            
            string sql = "SELECT article, product_name, unit, price, supplier, manufacturer, category, discount, stock_quantity, description, photo FROM products WHERE 1=1";
            
            if (!string.IsNullOrEmpty(search))
            {
                sql += " AND (product_name ILIKE @search OR manufacturer ILIKE @search OR article ILIKE @search)";
            }

            if (sortBy == "Стоимость (возрастание)")
                sql += " ORDER BY price ASC";
            else if (sortBy == "Стоимость (убывание)")
                sql += " ORDER BY price DESC";
            else if (sortBy == "Размер скидки")
                sql += " ORDER BY discount DESC";

            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    if (!string.IsNullOrEmpty(search))
                    {
                        cmd.Parameters.AddWithValue("@search", "%" + search + "%");
                    }

                    using (NpgsqlDataReader reader = (NpgsqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Product p = new Product();
                            p.Article = reader["article"].ToString();
                            p.ProductName = reader["product_name"].ToString();
                            p.Unit = reader["unit"].ToString();
                            p.Price = reader["price"] != DBNull.Value ? Convert.ToDecimal(reader["price"]) : 0;
                            p.Supplier = reader["supplier"].ToString();
                            p.Manufacturer = reader["manufacturer"].ToString();
                            p.Category = reader["category"].ToString();
                            p.Discount = reader["discount"] != DBNull.Value ? Convert.ToDecimal(reader["discount"]) : 0;
                            p.StockQuantity = reader["stock_quantity"] != DBNull.Value ? Convert.ToInt32(reader["stock_quantity"]) : 0;
                            p.Description = reader["description"].ToString();
                            p.Photo = reader["photo"].ToString();

                            list.Add(p);
                        }
                    }
                }
            }
            return list;
        }

        public static async Task<bool> SaveProductAsync(Product p, bool isNew)
        {
            string sql;
            if (isNew)
            {
                sql = "INSERT INTO products (article, product_name, unit, price, supplier, manufacturer, category, discount, stock_quantity, description, photo) " +
                      "VALUES (@article, @name, @unit, @price, @supplier, @manufacturer, @category, @discount, @stock, @desc, @photo)";
            }
            else
            {
                sql = "UPDATE products SET product_name=@name, unit=@unit, price=@price, supplier=@supplier, manufacturer=@manufacturer, " +
                      "category=@category, discount=@discount, stock_quantity=@stock, description=@desc, photo=@photo WHERE article=@article";
            }

            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@article", p.Article);
                    cmd.Parameters.AddWithValue("@name", p.ProductName);
                    cmd.Parameters.AddWithValue("@unit", p.Unit);
                    cmd.Parameters.AddWithValue("@price", p.Price);
                    cmd.Parameters.AddWithValue("@supplier", p.Supplier);
                    cmd.Parameters.AddWithValue("@manufacturer", p.Manufacturer);
                    cmd.Parameters.AddWithValue("@category", p.Category);
                    cmd.Parameters.AddWithValue("@discount", p.Discount);
                    cmd.Parameters.AddWithValue("@stock", p.StockQuantity);
                    cmd.Parameters.AddWithValue("@desc", p.Description);
                    cmd.Parameters.AddWithValue("@photo", p.Photo);

                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        public static async Task<bool> DeleteProductAsync(string article)
        {
            string sql = "DELETE FROM products WHERE article = @article";
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@article", article);
                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }

        public static async Task<DataTable> GetOrdersAsync()
        {
            DataTable table = new DataTable();
            string sql = "SELECT order_id, customer_name, order_date, delivery_date, order_status, pickup_code FROM orders ORDER BY order_id DESC";
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    using (NpgsqlDataReader reader = (NpgsqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        table.Load(reader);
                    }
                }
            }
            return table;
        }

        public static async Task<bool> UpdateOrderStatusAsync(int orderId, string status)
        {
            string sql = "UPDATE orders SET order_status = @status WHERE order_id = @orderId";
            using (NpgsqlConnection conn = new NpgsqlConnection(ConnectionString))
            {
                await conn.OpenAsync();
                using (NpgsqlCommand cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@orderId", orderId);
                    int rows = await cmd.ExecuteNonQueryAsync();
                    return rows > 0;
                }
            }
        }
    }
}