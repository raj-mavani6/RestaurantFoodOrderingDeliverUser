using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantFoodOrderingDeliverUser.Models
{
    public class DeliveryOrder
    {
        [Key]
        public int DeliveryOrderId { get; set; }
        
        public int OrderId { get; set; }
        
        public int DeliveryUserId { get; set; }
        
        [StringLength(100)]
        public string? CustomerName { get; set; }
        
        [StringLength(15)]
        public string? CustomerPhone { get; set; }
        
        [StringLength(500)]
        public string? DeliveryAddress { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        
        [StringLength(50)]
        public string? Status { get; set; }
        
        [StringLength(50)]
        public string? PaymentMethod { get; set; }
        
        [StringLength(50)]
        public string? PaymentStatus { get; set; }
        
        [StringLength(1000)]
        public string? SpecialInstructions { get; set; }
        
        public DateTime AssignedTime { get; set; }
        
        public DateTime? PickupTime { get; set; }
        
        public DateTime? DeliveredTime { get; set; }
        
        public DateTime? EstimatedDeliveryTime { get; set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal Distance { get; set; }
        
        [StringLength(500)]
        public string? DeliveryNotes { get; set; }
        
        public int? CustomerRating { get; set; }
        
        [StringLength(500)]
        public string? CustomerFeedback { get; set; }
        
        // Navigation property
        public virtual DeliveryUser? DeliveryUser { get; set; }
    }
}
