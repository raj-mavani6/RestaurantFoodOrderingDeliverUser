using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantFoodOrderingDeliverUser.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        
        public int CustomerId { get; set; }
        
        public int? DeliveryUserId { get; set; }
        
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }
        
        [StringLength(50)]
        public string? Status { get; set; }
        
        [StringLength(500)]
        public string? DeliveryAddress { get; set; }
        
        [StringLength(15)]
        public string? ContactPhone { get; set; }
        
        [StringLength(1000)]
        public string? SpecialInstructions { get; set; }
        
        public DateTime OrderDate { get; set; }
        
        public DateTime? PickupTime { get; set; }
        
        public DateTime? DeliveryDate { get; set; }
        
        public DateTime? EstimatedDeliveryTime { get; set; }
        
        [StringLength(50)]
        public string? PaymentMethod { get; set; }
        
        [StringLength(50)]
        public string? PaymentStatus { get; set; }
        
        [StringLength(500)]
        public string? DeliveryNotes { get; set; }
        
        // Navigation property
        public virtual DeliveryUser? DeliveryUser { get; set; }
        
        // Customer name (populated from join)
        [NotMapped]
        public string? CustomerName { get; set; }
    }
}
