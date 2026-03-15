using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantFoodOrderingDeliverUser.Models
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public int DeliveryUserId { get; set; }
        public DateTime AttendanceDate { get; set; }
        
        // Time entries (nullable since they may not be set initially)
        public string? CheckInTime { get; set; }                 // In Time (9:00 PM)
        public string? CheckOutTime { get; set; }                // Out Time (9:00 AM)
        public string? IntermediateStartTime { get; set; }       // Break Start (1:00 AM)
        public string? IntermediateEndTime { get; set; }         // Break End (2:00 AM)
        
        // Reasons for late/early entries (nullable since they're optional)
        public string? InTimeReason { get; set; }
        public string? OutTimeReason { get; set; }
        public string? IntermediateStartReason { get; set; }
        public string? IntermediateEndReason { get; set; }
        
        public string? Status { get; set; } // Present, Absent, Late, Half Day
        public string? Notes { get; set; }
        public int OrdersCompleted { get; set; }
        public double DistanceCovered { get; set; } // in kilometers
        
        // Calculated Properties
        [NotMapped]
        public string TotalWorkedHours
        {
            get
            {
                if (string.IsNullOrEmpty(CheckInTime) || string.IsNullOrEmpty(CheckOutTime)) return "--:--";
                try
                {
                    if (DateTime.TryParse(CheckInTime, out var inTime) && DateTime.TryParse(CheckOutTime, out var outTime))
                    {
                        var totalMinutes = (outTime - inTime).TotalMinutes;
                        if (totalMinutes < 0) totalMinutes += 24 * 60; // Handle cross-day shifts
                        
                        // Subtract break
                        if (!string.IsNullOrEmpty(IntermediateStartTime) && !string.IsNullOrEmpty(IntermediateEndTime))
                        {
                            if (DateTime.TryParse(IntermediateStartTime, out var bStart) && DateTime.TryParse(IntermediateEndTime, out var bEnd))
                            {
                                var breakMinutes = (bEnd - bStart).TotalMinutes;
                                if (breakMinutes < 0) breakMinutes += 24 * 60;
                                totalMinutes -= breakMinutes;
                            }
                        }
                        
                        var h = (int)(totalMinutes / 60);
                        var m = (int)(totalMinutes % 60);
                        return $"{h}h {m}m";
                    }
                }
                catch { }
                return "--:--";
            }
        }

        [NotMapped]
        public string BreakDuration
        {
            get
            {
                if (string.IsNullOrEmpty(IntermediateStartTime) || string.IsNullOrEmpty(IntermediateEndTime)) return "--:--";
                try
                {
                    if (DateTime.TryParse(IntermediateStartTime, out var bStart) && DateTime.TryParse(IntermediateEndTime, out var bEnd))
                    {
                        var diff = (bEnd - bStart).TotalMinutes;
                        if (diff < 0) diff += 24 * 60;
                        var h = (int)(diff / 60);
                        var m = (int)(diff % 60);
                        return $"{h}h {m}m";
                    }
                }
                catch { }
                return "--:--";
            }
        }

        // Navigation Property
        public virtual DeliveryUser? DeliveryUser { get; set; }
    }
}
