using Microsoft.EntityFrameworkCore;
using RestaurantFoodOrderingDeliverUser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantFoodOrderingDeliverUser.Data
{
    public interface IDeliveryEarningRepository : IRepository<DeliveryEarning>
    {
        Task<IEnumerable<DeliveryEarning>> GetUserEarningsAsync(int userId);
        Task<IEnumerable<DeliveryEarning>> GetUserEarningsForMonthAsync(int userId, int month, int year);
        Task<decimal> GetTotalEarningsAsync(int userId);
        Task<decimal> GetMonthlyEarningsAsync(int userId, int month, int year);
        Task<IEnumerable<DeliveryEarning>> GetPendingEarningsAsync(int userId);
    }

    public class DeliveryEarningRepository : Repository<DeliveryEarning>, IDeliveryEarningRepository
    {
        private readonly ApplicationDbContext _context;

        public DeliveryEarningRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DeliveryEarning>> GetUserEarningsAsync(int userId)
        {
            return await _context.DeliveryEarnings
                .Where(e => e.DeliveryUserId == userId)
                .OrderByDescending(e => e.EarningDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<DeliveryEarning>> GetUserEarningsForMonthAsync(int userId, int month, int year)
        {
            return await _context.DeliveryEarnings
                .Where(e => e.DeliveryUserId == userId 
                    && e.EarningDate.Month == month 
                    && e.EarningDate.Year == year)
                .OrderByDescending(e => e.EarningDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalEarningsAsync(int userId)
        {
            return await _context.DeliveryEarnings
                .Where(e => e.DeliveryUserId == userId && e.PaymentStatus == "Paid")
                .SumAsync(e => e.TotalEarning);
        }

        public async Task<decimal> GetMonthlyEarningsAsync(int userId, int month, int year)
        {
            return await _context.DeliveryEarnings
                .Where(e => e.DeliveryUserId == userId 
                    && e.EarningDate.Month == month 
                    && e.EarningDate.Year == year
                    && e.PaymentStatus == "Paid")
                .SumAsync(e => e.TotalEarning);
        }

        public async Task<IEnumerable<DeliveryEarning>> GetPendingEarningsAsync(int userId)
        {
            return await _context.DeliveryEarnings
                .Where(e => e.DeliveryUserId == userId && e.PaymentStatus == "Pending")
                .OrderByDescending(e => e.EarningDate)
                .ToListAsync();
        }
    }
}
