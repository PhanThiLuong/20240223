using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using TheFirstApp.Models;

namespace TheFirstApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly string connectionString;
        public ProductsController(IConfiguration configuration) 
        {
            connectionString = configuration["ConnectionStrings:SqlServerDb"]??"";
        }
        [HttpPost]
        public IActionResult CreateProduct(ProductsDto productsDto)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string sql = " INSERT INTO products " +
                        "(name, category, price, description) VALUES" +
                        "(@name, @category, @price, @description)";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", productsDto.Name);
                        command.Parameters.AddWithValue("@category", productsDto.Category);
                        command.Parameters.AddWithValue("@price", productsDto.Price);
                        command.Parameters.AddWithValue("@description", productsDto.Description);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Product", "Sorry, but we have an exception");
                return BadRequest(ModelState);     
            }

            return Ok();
        }

        [HttpGet]
        public IActionResult GetProducts()
        {
            List<Product> products = new List<Product>();
            try 
            {
               using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM products";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Product product = new Product();

                                product.Id = reader.GetInt32(0);
                                product.Name = reader.GetString(1);
                                product.Category = reader.GetString(2);
                                product.Price = reader.GetDecimal(3);
                                product.Description = reader.GetString(4);
                                product.CreatedAt = reader.GetDateTime(5);

                                products.Add(product);

                            }
                        }
                    }

                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("Product", "Sorry, but we have an exception");
                return BadRequest(ModelState);


            }
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetProduct(int id)
        {
            Product product = new Product();
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM products WHERE id =@id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                product.Id = reader.GetInt32(0);
                                product.Name = reader.GetString(1);
                                product.Category = reader.GetString(2);
                                product.Price = reader.GetDecimal(3);
                                product.Description = reader.GetString(4);
                                product.CreatedAt = reader.GetDateTime(5);
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Product", "Sorry, but we have an exception");
                return BadRequest(ModelState);
            }
            return Ok(product);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, ProductsDto productsDto)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {

                            connection.Open();
                    string sql = " UPDATE products SET name=@name," +
                        " category=@category, price=@price, description =@description WHERE id=@id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@name", productsDto.Name);
                        command.Parameters.AddWithValue("@category", productsDto.Category);
                        command.Parameters.AddWithValue("@price", productsDto.Price);
                        command.Parameters.AddWithValue("@description", productsDto.Description);
                        command.Parameters.AddWithValue("@id", id);

                        command.ExecuteNonQuery();
                    }

                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("Product", "Sorry, but we have an exception");
                return BadRequest(ModelState);
            }
            return Ok();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            try
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM products WHERE id=@id";
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }

                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("Product", "Sorry, but we have an exception");
                return BadRequest(ModelState);
            }

            return Ok();
        }
    }
}
