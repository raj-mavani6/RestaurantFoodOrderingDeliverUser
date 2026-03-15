using Microsoft.EntityFrameworkCore;
using RestaurantFoodOrderingDeliverUser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantFoodOrderingDeliverUser.Data
{
    public interface IAttendanceRepository : IRepository<Attendance>
    {
        Task<Attendance?> GetTodayAttendanceAsync(int deliveryUserId);
        Task<IEnumerable<Attendance>> GetMonthlyAttendanceAsync(int deliveryUserId, int month, int year);
        Task<IEnumerable<Attendance>> GetUserAttendanceAsync(int deliveryUserId, int limit = 30);
        Task<Attendance?> GetLatestAttendanceAsync(int deliveryUserId);
        Task<Attendance> CreateOrGetTodayAttendanceAsync(int deliveryUserId);
        Task UpdateInTimeAsync(int deliveryUserId, string? time, string? reason);
        Task UpdateIntermediateStartAsync(int deliveryUserId, string? time, string? reason);
        Task UpdateIntermediateEndAsync(int deliveryUserId, string? time, string? reason);
        Task UpdateOutTimeAsync(int deliveryUserId, string? time, string? reason);
    }

    public class AttendanceRepository : Repository<Attendance>, IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // Get today's attendance record
        public async Task<Attendance?> GetTodayAttendanceAsync(int deliveryUserId)
        {
            var today = DateTime.Now.Date;
            return await _context.Attendances
                .FirstOrDefaultAsync(a => a.DeliveryUserId == deliveryUserId && 
                                         a.AttendanceDate.Date == today);
        }

        // Create or get today's attendance record
        public async Task<Attendance> CreateOrGetTodayAttendanceAsync(int deliveryUserId)
        {
            var today = DateTime.Now.Date;
            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.DeliveryUserId == deliveryUserId && 
                                         a.AttendanceDate.Date == today);
            
            if (attendance == null)
            {
                attendance = new Attendance
                {
                    DeliveryUserId = deliveryUserId,
                    AttendanceDate = today,
                    Status = "Present"
                };
                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();
            }
            
            return attendance;
        }

        // Update In Time
        public async Task UpdateInTimeAsync(int deliveryUserId, string? time, string? reason)
        {
            var attendance = await CreateOrGetTodayAttendanceAsync(deliveryUserId);
            attendance.CheckInTime = time;
            attendance.InTimeReason = reason;
            
            // Update status based on time (if late, mark as Late)
            if (!string.IsNullOrEmpty(reason))
            {
                attendance.Status = "Late";
            }
            
            await _context.SaveChangesAsync();
        }

        // Update Intermediate Start Time
        public async Task UpdateIntermediateStartAsync(int deliveryUserId, string? time, string? reason)
        {
            var attendance = await CreateOrGetTodayAttendanceAsync(deliveryUserId);
            attendance.IntermediateStartTime = time;
            attendance.IntermediateStartReason = reason;
            await _context.SaveChangesAsync();
        }

        // Update Intermediate End Time
        public async Task UpdateIntermediateEndAsync(int deliveryUserId, string? time, string? reason)
        {
            var attendance = await CreateOrGetTodayAttendanceAsync(deliveryUserId);
            attendance.IntermediateEndTime = time;
            attendance.IntermediateEndReason = reason;
            await _context.SaveChangesAsync();
        }

        // Update Out Time
        public async Task UpdateOutTimeAsync(int deliveryUserId, string? time, string? reason)
        {
            var attendance = await CreateOrGetTodayAttendanceAsync(deliveryUserId);
            attendance.CheckOutTime = time;
            attendance.OutTimeReason = reason;
            await _context.SaveChangesAsync();
        }

        // Get monthly attendance records
        public async Task<IEnumerable<Attendance>> GetMonthlyAttendanceAsync(int deliveryUserId, int month, int year)
        {
            return await _context.Attendances
                .Where(a => a.DeliveryUserId == deliveryUserId &&
                           a.AttendanceDate.Month == month &&
                           a.AttendanceDate.Year == year)
                .OrderByDescending(a => a.AttendanceDate)
                .ToListAsync();
        }

        // Get user's attendance records with limit
        public async Task<IEnumerable<Attendance>> GetUserAttendanceAsync(int deliveryUserId, int limit = 30)
        {
            return await _context.Attendances
                .Where(a => a.DeliveryUserId == deliveryUserId)
                .OrderByDescending(a => a.AttendanceDate)
                .Take(limit)
                .ToListAsync();
        }

        // Get latest attendance record
        public async Task<Attendance?> GetLatestAttendanceAsync(int deliveryUserId)
        {
            return await _context.Attendances
                .Where(a => a.DeliveryUserId == deliveryUserId)
                .OrderByDescending(a => a.AttendanceDate)
                .FirstOrDefaultAsync();
        }
    }
}
