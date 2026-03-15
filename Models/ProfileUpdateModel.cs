using System;

namespace RestaurantFoodOrderingDeliverUser.Models
{
    public class ProfileUpdateModel
    {
        // Basic Info
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        
        // Personal Info
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? FatherName { get; set; }
        public string? AadharNumber { get; set; }
        public string? PANNumber { get; set; }
        public string? PermanentAddress { get; set; }
        public string? CurrentAddress { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }
        
        // Medical Info
        public string? BloodGroup { get; set; }
        public string? Height { get; set; }
        public string? Weight { get; set; }
        public string? Vision { get; set; }
        public string? KnownAllergies { get; set; }
        public string? ChronicConditions { get; set; }
        public DateTime? LastHealthCheckup { get; set; }
        public DateTime? MedicalInsuranceExpiry { get; set; }
        
        // Vehicle Info
        public string? VehicleType { get; set; }
        public string? VehicleModel { get; set; }
        public string? VehicleNumber { get; set; }
        public string? VehicleColor { get; set; }
        public int? VehicleManufacturingYear { get; set; }
        public DateTime? VehicleInsuranceExpiry { get; set; }
        public DateTime? PUCExpiry { get; set; }
        public DateTime? FitnessCertificateExpiry { get; set; }
        
        // Documents
        public string? DrivingLicenseNumber { get; set; }
        public DateTime? DrivingLicenseExpiry { get; set; }
        public string? VehicleInsuranceNumber { get; set; }
        public string? PUCNumber { get; set; }
        public string? MedicalInsuranceNumber { get; set; }
        
        // Bank Details
        public string? BankAccountHolderName { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankIFSCCode { get; set; }
        public string? BankBranch { get; set; }
        public string? UPIID { get; set; }
        
        // Emergency Contact
        public string? EmergencyContactName { get; set; }
        public string? EmergencyContactPhone { get; set; }
        public string? EmergencyContactRelation { get; set; }
        public string? SecondaryContactName { get; set; }
        public string? SecondaryContactPhone { get; set; }
        public string? SecondaryContactRelation { get; set; }
        public string? EmergencyAddress { get; set; }
    }
}
