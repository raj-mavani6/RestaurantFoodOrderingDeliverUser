using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantFoodOrderingDeliverUser.Models
{
    public class DeliveryEarning
    {
        [Key]
        public int EarningId { get; set; }
        
        public int DeliveryUserId { get; set; }
        
        public int? DeliveryOrderId { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal DeliveryFee { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal TipAmount { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Bonus { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Incentive { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal Deduction { get; set; }
        
        [StringLength(200)]
        public string? DeductionReason { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalEarning { get; set; }
        
        [StringLength(50)]
        public string? EarningType { get; set; } // Delivery, Bonus, Incentive, Adjustment
        
        [StringLength(200)]
        public string? Description { get; set; }
        
        public DateTime EarningDate { get; set; }
        
        [StringLength(50)]
        public string? PaymentStatus { get; set; } // Pending, Paid, Processing
        
        public DateTime? PaidDate { get; set; }
        
        [StringLength(50)]
        public string? PaymentMethod { get; set; } // Bank Transfer, UPI, Cash
        
        [StringLength(100)]
        public string? TransactionId { get; set; }
        
        // Navigation properties
        public virtual DeliveryUser? DeliveryUser { get; set; }
        public virtual DeliveryOrder? DeliveryOrder { get; set; }
    }
}
