using Microsoft.EntityFrameworkCore;
using RestaurantFoodOrderingDeliverUser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantFoodOrderingDeliverUser.Data
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAssignedOrdersAsync(int deliveryUserId);
        Task<List<Order>> GetActiveOrdersAsync(int deliveryUserId);
        Task<List<Order>> GetDeliveredOrdersAsync(int deliveryUserId, int count = 30);
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task UpdateOrderStatusAsync(int orderId, string status);
        Task MarkAsPickedUpAsync(int orderId);
        Task MarkAsDeliveredAsync(int orderId);
        Task<int> GetTodayDeliveryCountAsync(int deliveryUserId);
        Task<decimal> GetTodayEarningsAsync(int deliveryUserId);
    }

    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all orders assigned to delivery user
        public async Task<List<Order>> GetAssignedOrdersAsync(int deliveryUserId)
        {
            return await _context.Orders
                .Where(o => o.DeliveryUserId == deliveryUserId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        // Get active orders (not delivered)
        public async Task<List<Order>> GetActiveOrdersAsync(int deliveryUserId)
        {
            return await _context.Orders
                .Where(o => o.DeliveryUserId == deliveryUserId && 
                           o.Status != "Delivered" && 
                           o.Status != "Cancelled")
                .OrderBy(o => o.EstimatedDeliveryTime)
                .ToListAsync();
        }

        // Get delivered orders history
        public async Task<List<Order>> GetDeliveredOrdersAsync(int deliveryUserId, int count = 30)
        {
            return await _context.Orders
                .Where(o => o.DeliveryUserId == deliveryUserId && o.Status == "Delivered")
                .OrderByDescending(o => o.DeliveryDate)
                .Take(count)
                .ToListAsync();
        }

        // Get order by ID
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        // Update order status
        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        // Mark order as picked up
        public async Task MarkAsPickedUpAsync(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order != null)
            {
                order.Status = "Picked Up";
                order.PickupTime = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        // Mark order as delivered
        public async Task MarkAsDeliveredAsync(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order != null)
            {
                order.Status = "Delivered";
                order.DeliveryDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        // Get today's delivery count
        public async Task<int> GetTodayDeliveryCountAsync(int deliveryUserId)
        {
            var today = DateTime.Today;
            return await _context.Orders
                .CountAsync(o => o.DeliveryUserId == deliveryUserId && 
                                o.Status == "Delivered" &&
                                o.DeliveryDate.HasValue &&
                                o.DeliveryDate.Value.Date == today);
        }

        // Get today's earnings (10% of total amount as commission)
        public async Task<decimal> GetTodayEarningsAsync(int deliveryUserId)
        {
            var today = DateTime.Today;
            var totalAmount = await _context.Orders
                .Where(o => o.DeliveryUserId == deliveryUserId && 
                           o.Status == "Delivered" &&
                           o.DeliveryDate.HasValue &&
                           o.DeliveryDate.Value.Date == today)
                .SumAsync(o => o.TotalAmount);
            
            // 10% delivery commission
            return totalAmount * 0.10m;
        }
    }
}
