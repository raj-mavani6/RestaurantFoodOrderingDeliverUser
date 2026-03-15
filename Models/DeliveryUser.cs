using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RestaurantFoodOrderingDeliverUser.Models
{
    public class DeliveryUser
    {
        [Key]
        public int DeliveryUserId { get; set; }
        
        // Basic Information
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(15)]
        public string Phone { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? ProfilePhoto { get; set; }
        
        [StringLength(50)]
        public string Status { get; set; } = "Active";
        
        [Column(TypeName = "decimal(3,1)")]
        public decimal Rating { get; set; }
        
        public int TotalDeliveries { get; set; }
        
        [Column(TypeName = "decimal(12,2)")]
        public decimal TotalEarnings { get; set; }
        
        public DateTime JoinDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Personal Information
        public DateTime? DateOfBirth { get; set; }
        
        [StringLength(20)]
        public string? Gender { get; set; }
        
        [StringLength(100)]
        public string? FatherName { get; set; }
        
        [StringLength(20)]
        public string? AadharNumber { get; set; }
        
        [StringLength(15)]
        public string? PANNumber { get; set; }
        
        [StringLength(500)]
        public string? PermanentAddress { get; set; }
        
        [StringLength(500)]
        public string? CurrentAddress { get; set; }
        
        [StringLength(100)]
        public string? City { get; set; }
        
        [StringLength(100)]
        public string? State { get; set; }
        
        [StringLength(10)]
        public string? Pincode { get; set; }
        
        // Medical Information
        [StringLength(10)]
        public string? BloodGroup { get; set; }
        
        [StringLength(20)]
        public string? Height { get; set; }
        
        [StringLength(20)]
        public string? Weight { get; set; }
        
        [StringLength(50)]
        public string? Vision { get; set; }
        
        [StringLength(500)]
        public string? KnownAllergies { get; set; }
        
        [StringLength(500)]
        public string? ChronicConditions { get; set; }
        
        public DateTime? LastHealthCheckup { get; set; }
        
        [StringLength(50)]
        public string? MedicalInsuranceNumber { get; set; }
        
        public DateTime? MedicalInsuranceExpiry { get; set; }
        
        // Vehicle Information
        [StringLength(50)]
        public string? VehicleType { get; set; }
        
        [StringLength(100)]
        public string? VehicleModel { get; set; }
        
        [StringLength(20)]
        public string? VehicleNumber { get; set; }
        
        [StringLength(50)]
        public string? VehicleColor { get; set; }
        
        public int? VehicleManufacturingYear { get; set; }
        
        [StringLength(50)]
        public string? VehicleInsuranceNumber { get; set; }
        
        public DateTime? VehicleInsuranceExpiry { get; set; }
        
        [StringLength(50)]
        public string? PUCNumber { get; set; }
        
        public DateTime? PUCExpiry { get; set; }
        
        public DateTime? FitnessCertificateExpiry { get; set; }
        
        // Documents
        [StringLength(50)]
        public string? DrivingLicenseNumber { get; set; }
        
        public DateTime? DrivingLicenseExpiry { get; set; }
        
        public bool DrivingLicenseVerified { get; set; }
        public bool AadharVerified { get; set; }
        public bool PANVerified { get; set; }
        public bool VehicleRCVerified { get; set; }
        public bool VehicleInsuranceVerified { get; set; }
        public bool MedicalCertificateVerified { get; set; }
        
        // Bank Details
        [StringLength(100)]
        public string? BankAccountHolderName { get; set; }
        
        [StringLength(100)]
        public string? BankName { get; set; }
        
        [StringLength(30)]
        public string? BankAccountNumber { get; set; }
        
        [StringLength(15)]
        public string? BankIFSCCode { get; set; }
        
        [StringLength(100)]
        public string? BankBranch { get; set; }
        
        [StringLength(100)]
        public string? UPIID { get; set; }
        
        // Emergency Contact
        [StringLength(100)]
        public string? EmergencyContactName { get; set; }
        
        [StringLength(15)]
        public string? EmergencyContactPhone { get; set; }
        
        [StringLength(50)]
        public string? EmergencyContactRelation { get; set; }
        
        [StringLength(100)]
        public string? SecondaryContactName { get; set; }
        
        [StringLength(15)]
        public string? SecondaryContactPhone { get; set; }
        
        [StringLength(50)]
        public string? SecondaryContactRelation { get; set; }
        
        [StringLength(500)]
        public string? EmergencyAddress { get; set; }

        // Navigation Properties
        public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
        public virtual ICollection<Leave> Leaves { get; set; } = new List<Leave>();
    }
}
