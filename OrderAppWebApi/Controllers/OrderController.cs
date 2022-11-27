using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MySqlX.XDevAPI.Common;
using OrderAppWebApi.Context;
using OrderAppWebApi.Models.Dtos;
using OrderAppWebApi.Models.Entites;
using OrderAppWebApi.Models.Results;
using OrderAppWebApi.RabbitMq;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System.Text;

namespace OrderAppWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly OrderContext _context;
        private readonly IMapper _mapper;

        public OrderController(IMemoryCache memoryCache, OrderContext context, IMapper mapper)
        {
            _memoryCache = memoryCache;
            _context = context;
            _mapper = mapper;
        }

        #region Logged in Memory-Cache 

        //[HttpGet]
        //public async Task<IActionResult> Get(string? category)
        //{
        //    var result = new List<Product>();
        //    if (category == null)
        //    {
        //        result = _memoryCache.Get("products") as List<Product>;
        //        if (result == null)
        //        {
        //            result = await _context.Products.ToListAsync();
        //            _memoryCache.Set("products", result, TimeSpan.FromMinutes(10));
        //        }
        //    }
        //    else
        //    {
        //        result = _memoryCache.Get($"products-{category}") as List<Product>;
        //        if (result == null)
        //        {
        //            result = await _context.Products.Where(p => p.Category == category).ToListAsync();
        //            _memoryCache.Set($"products-{category}", result, TimeSpan.FromMinutes(10));
        //        }
        //    }
        //    var productDtos = _mapper.Map<List<Product>, List<ProductDto>>(result);
        //    // Auto-Mapper aracılığıyla Product türünden ProductDto'ya dönüştürülmüştür.

        //    return Ok(new ApiResponse<List<ProductDto>>(StatusType.Success, productDtos));
        //}

        #endregion

        #region Logged in Redis
        [HttpGet]
        public async Task<IActionResult> Get(string? category)
        {
            var redisclient = new RedisClient("localhost", 6379);
            IRedisTypedClient<List<Product>> redisProducts = redisclient.As<List<Product>>();

            var result = new List<Product>();
            if (category == null)
            {
                result = redisclient.Get<List<Product>>("products");
                if (result == null)
                {
                    result = await _context.Products.ToListAsync();
                    redisclient.Set("products", result, TimeSpan.FromMinutes(10));
                }
            }
            else
            {
                result = redisclient.Get<List<Product>>($"products-{category}");
                if (result == null)
                {
                    result = await _context.Products.Where(p => p.Category == category).ToListAsync();
                    redisclient.Set($"products-{category}", result, TimeSpan.FromMinutes(10));
                }
            }
            var productDtos = _mapper.Map<List<Product>, List<ProductDto>>(result);
            // Auto-Mapper aracılığıyla Product türünden ProductDto'ya dönüştürülmüştür.

            return Ok(new ApiResponse<List<ProductDto>>(StatusType.Success, productDtos));
        }
        #endregion

        #region Create Order
        [Route("/api/CreateOrder")]
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest createOrderRequest)
        {
            Order order = _mapper.Map<CreateOrderRequest, Order>(createOrderRequest);
            List<OrderDetail> orderDetails = _mapper.Map<List<ProductDetailDto>, List<OrderDetail>>(createOrderRequest.ProductDetails) as List<OrderDetail>;

            order.TotalAmount = createOrderRequest.ProductDetails.Sum(p => p.Amount);
            order.OrderDetails = orderDetails;
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();


            // Mail işlemi yapılacak.
            var data = Encoding.UTF8.GetBytes(createOrderRequest.CustomerEmail);
            SetQueues.SendQueue(data);

            return Ok(new ApiResponse<Order>(StatusType.Success, order));
        }
        #endregion

        #region Add 1000 Products

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            for (int i = 0; i < 1000; i++)
            {
                Product product = new()
                {
                    Category = $"Category {i}",
                    CreatedDate = DateTime.Now,
                    Description = "Description",
                    Status = true,
                    Unit = i,
                    UnitPrice = i * 10
                };

                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
            }
            return Ok("Added Products");
        }

        #endregion

    }
}
