using Microsoft.EntityFrameworkCore;
using RestaurantFoodOrderingDeliverUser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantFoodOrderingDeliverUser.Data
{
    public interface IDeliveryOrderRepository
    {
        Task<List<DeliveryOrder>> GetAssignedOrdersAsync(int deliveryUserId);
        Task<List<DeliveryOrder>> GetActiveOrdersAsync(int deliveryUserId);
        Task<List<DeliveryOrder>> GetDeliveredOrdersAsync(int deliveryUserId, int count = 30);
        Task<DeliveryOrder?> GetOrderByIdAsync(int deliveryOrderId);
        Task UpdateOrderStatusAsync(int deliveryOrderId, string status);
        Task MarkAsPickedUpAsync(int deliveryOrderId);
        Task MarkAsDeliveredAsync(int deliveryOrderId);
        Task<int> GetTodayDeliveryCountAsync(int deliveryUserId);
        Task<decimal> GetTodayEarningsAsync(int deliveryUserId);
    }

    public class DeliveryOrderRepository : IDeliveryOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public DeliveryOrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get all orders assigned to delivery user
        public async Task<List<DeliveryOrder>> GetAssignedOrdersAsync(int deliveryUserId)
        {
            return await _context.DeliveryOrders
                .Where(o => o.DeliveryUserId == deliveryUserId)
                .OrderByDescending(o => o.AssignedTime)
                .ToListAsync();
        }

        // Get active orders (not delivered)
        public async Task<List<DeliveryOrder>> GetActiveOrdersAsync(int deliveryUserId)
        {
            return await _context.DeliveryOrders
                .Where(o => o.DeliveryUserId == deliveryUserId && 
                           o.Status != "Delivered" && 
                           o.Status != "Cancelled")
                .OrderBy(o => o.EstimatedDeliveryTime)
                .ToListAsync();
        }

        // Get delivered orders history
        public async Task<List<DeliveryOrder>> GetDeliveredOrdersAsync(int deliveryUserId, int count = 30)
        {
            return await _context.DeliveryOrders
                .Where(o => o.DeliveryUserId == deliveryUserId && o.Status == "Delivered")
                .OrderByDescending(o => o.DeliveredTime)
                .Take(count)
                .ToListAsync();
        }

        // Get order by ID
        public async Task<DeliveryOrder?> GetOrderByIdAsync(int deliveryOrderId)
        {
            return await _context.DeliveryOrders.FirstOrDefaultAsync(o => o.DeliveryOrderId == deliveryOrderId);
        }

        // Update order status
        public async Task UpdateOrderStatusAsync(int deliveryOrderId, string status)
        {
            var order = await _context.DeliveryOrders.FirstOrDefaultAsync(o => o.DeliveryOrderId == deliveryOrderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
        }

        // Mark order as picked up
        public async Task MarkAsPickedUpAsync(int deliveryOrderId)
        {
            var order = await _context.DeliveryOrders.FirstOrDefaultAsync(o => o.DeliveryOrderId == deliveryOrderId);
            if (order != null)
            {
                order.Status = "Picked Up";
                order.PickupTime = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        // Mark order as delivered and create earning record
        public async Task MarkAsDeliveredAsync(int deliveryOrderId)
        {
            var order = await _context.DeliveryOrders.FirstOrDefaultAsync(o => o.DeliveryOrderId == deliveryOrderId);
            if (order != null)
            {
                order.Status = "Delivered";
                order.DeliveredTime = DateTime.Now;
                
                // Create earning record for this delivery
                var earning = new DeliveryEarning
                {
                    DeliveryUserId = order.DeliveryUserId,
                    DeliveryOrderId = order.DeliveryOrderId,
                    DeliveryFee = 30.00m, // Base delivery fee
                    TipAmount = 0.00m, // Tips can be added later
                    Bonus = 0.00m,
                    Incentive = 0.00m,
                    Deduction = 0.00m,
                    TotalEarning = 30.00m, // Base delivery fee
                    EarningType = "Delivery",
                    Description = $"Order #{order.OrderId} - {order.CustomerName}",
                    EarningDate = DateTime.Now,
                    PaymentStatus = "Pending", // Will be paid later
                    PaidDate = null,
                    PaymentMethod = null,
                    TransactionId = null
                };
                
                _context.DeliveryEarnings.Add(earning);
                await _context.SaveChangesAsync();
            }
        }

        // Get today's delivery count
        public async Task<int> GetTodayDeliveryCountAsync(int deliveryUserId)
        {
            var today = DateTime.Today;
            return await _context.DeliveryOrders
                .CountAsync(o => o.DeliveryUserId == deliveryUserId && 
                                o.Status == "Delivered" &&
                                o.DeliveredTime.HasValue &&
                                o.DeliveredTime.Value.Date == today);
        }

        // Get today's earnings (10% of total amount as commission)
        public async Task<decimal> GetTodayEarningsAsync(int deliveryUserId)
        {
            var today = DateTime.Today;
            var totalAmount = await _context.DeliveryOrders
                .Where(o => o.DeliveryUserId == deliveryUserId && 
                           o.Status == "Delivered" &&
                           o.DeliveredTime.HasValue &&
                           o.DeliveredTime.Value.Date == today)
                .SumAsync(o => o.TotalAmount);
            
            // 10% delivery commission
            return totalAmount * 0.10m;
        }
    }
}
