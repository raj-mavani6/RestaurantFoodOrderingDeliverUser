using Microsoft.EntityFrameworkCore;
using RestaurantFoodOrderingDeliverUser.Models;

namespace RestaurantFoodOrderingDeliverUser.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets for Delivery User Panel
        public DbSet<DeliveryUser> DeliveryUsers { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Leave> Leaves { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<DeliveryOrder> DeliveryOrders { get; set; }
        public DbSet<DeliveryEarning> DeliveryEarnings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure DeliveryUser entity
            modelBuilder.Entity<DeliveryUser>(entity =>
            {
                entity.ToTable("DeliveryUsers"); // Explicit table name to match MySQL database
                entity.HasKey(e => e.DeliveryUserId);
                
                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);
                
                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(15);
                
                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(255);
                
                entity.Property(e => e.CurrentAddress)
                    .HasMaxLength(500);
                
                entity.Property(e => e.City)
                    .HasMaxLength(100);
                
                entity.Property(e => e.VehicleType)
                    .HasMaxLength(50);
                
                entity.Property(e => e.VehicleNumber)
                    .HasMaxLength(20);
                
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Active");
                
                entity.Property(e => e.Rating)
                    .HasPrecision(3, 1)
                    .HasDefaultValue(0);
                
                entity.Property(e => e.TotalDeliveries)
                    .HasDefaultValue(0);
                
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
                
                // Relationships
                entity.HasMany(e => e.Attendances)
                    .WithOne(a => a.DeliveryUser)
                    .HasForeignKey(a => a.DeliveryUserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasMany(e => e.Leaves)
                    .WithOne(l => l.DeliveryUser)
                    .HasForeignKey(l => l.DeliveryUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Attendance entity
            modelBuilder.Entity<Attendance>(entity =>
            {
                entity.ToTable("Attendance"); // Explicit table name to match MySQL database
                entity.HasKey(e => e.AttendanceId);
                
                entity.Property(e => e.AttendanceDate)
                    .IsRequired();
                
                entity.Property(e => e.CheckInTime)
                    .HasMaxLength(20);
                
                entity.Property(e => e.CheckOutTime)
                    .HasMaxLength(20);
                
                entity.Property(e => e.IntermediateStartTime)
                    .HasMaxLength(20);
                
                entity.Property(e => e.IntermediateEndTime)
                    .HasMaxLength(20);
                
                entity.Property(e => e.InTimeReason)
                    .HasMaxLength(500);
                
                entity.Property(e => e.OutTimeReason)
                    .HasMaxLength(500);
                
                entity.Property(e => e.IntermediateStartReason)
                    .HasMaxLength(500);
                
                entity.Property(e => e.IntermediateEndReason)
                    .HasMaxLength(500);
                
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Present");
                
                entity.Property(e => e.Notes)
                    .HasMaxLength(500);
                
                entity.Property(e => e.OrdersCompleted)
                    .HasDefaultValue(0);
                
                entity.Property(e => e.DistanceCovered)
                    .HasPrecision(8, 2)
                    .HasDefaultValue(0);
                
                // Relationships
                entity.HasOne(e => e.DeliveryUser)
                    .WithMany(d => d.Attendances)
                    .HasForeignKey(e => e.DeliveryUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Leave entity
            modelBuilder.Entity<Leave>(entity =>
            {
                entity.ToTable("Leaves"); // Explicit table name to match MySQL database
                entity.HasKey(e => e.LeaveId);
                
                entity.Property(e => e.StartDate)
                    .IsRequired();
                
                entity.Property(e => e.EndDate)
                    .IsRequired();
                
                entity.Property(e => e.LeaveType)
                    .IsRequired()
                    .HasMaxLength(50);
                
                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(500);
                
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Pending");
                
                entity.Property(e => e.ApprovedBy)
                    .HasMaxLength(100);
                
                entity.Property(e => e.AdminNotes)
                    .HasMaxLength(500);
                
                // Relationships
                entity.HasOne(e => e.DeliveryUser)
                    .WithMany(d => d.Leaves)
                    .HasForeignKey(e => e.DeliveryUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders"); // Explicit table name to match MySQL database
                entity.HasKey(e => e.OrderId);
                
                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(10,2)");
                
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Pending");
                
                entity.Property(e => e.DeliveryAddress)
                    .HasMaxLength(500);
                
                entity.Property(e => e.ContactPhone)
                    .HasMaxLength(15);
                
                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50);
                
                entity.Property(e => e.PaymentStatus)
                    .HasMaxLength(50);
                
                // Relationships
                entity.HasOne(e => e.DeliveryUser)
                    .WithMany()
                    .HasForeignKey(e => e.DeliveryUserId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Configure DeliveryOrder entity
            modelBuilder.Entity<DeliveryOrder>(entity =>
            {
                entity.ToTable("DeliveryOrders"); // Explicit table name
                entity.HasKey(e => e.DeliveryOrderId);
                
                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(10,2)");
                
                entity.Property(e => e.Distance)
                    .HasColumnType("decimal(5,2)");
                
                entity.Property(e => e.Status)
                    .HasMaxLength(50)
                    .HasDefaultValue("Assigned");
                
                entity.Property(e => e.CustomerName)
                    .HasMaxLength(100);
                
                entity.Property(e => e.CustomerPhone)
                    .HasMaxLength(15);
                
                entity.Property(e => e.DeliveryAddress)
                    .HasMaxLength(500);
                
                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50);
                
                entity.Property(e => e.PaymentStatus)
                    .HasMaxLength(50);
                
                // Relationships
                entity.HasOne(e => e.DeliveryUser)
                    .WithMany()
                    .HasForeignKey(e => e.DeliveryUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure DeliveryEarning entity
            modelBuilder.Entity<DeliveryEarning>(entity =>
            {
                entity.ToTable("DeliveryEarning");
                entity.HasKey(e => e.EarningId);
                
                entity.Property(e => e.DeliveryFee)
                    .HasColumnType("decimal(10,2)");
                
                entity.Property(e => e.TipAmount)
                    .HasColumnType("decimal(10,2)");
                
                entity.Property(e => e.Bonus)
                    .HasColumnType("decimal(10,2)");
                
                entity.Property(e => e.Incentive)
                    .HasColumnType("decimal(10,2)");
                
                entity.Property(e => e.Deduction)
                    .HasColumnType("decimal(10,2)");
                
                entity.Property(e => e.TotalEarning)
                    .HasColumnType("decimal(10,2)");
                
                entity.Property(e => e.EarningType)
                    .HasMaxLength(50);
                
                entity.Property(e => e.Description)
                    .HasMaxLength(200);
                
                entity.Property(e => e.PaymentStatus)
                    .HasMaxLength(50);
                
                entity.Property(e => e.PaymentMethod)
                    .HasMaxLength(50);
                
                entity.Property(e => e.TransactionId)
                    .HasMaxLength(100);
                
                // Relationships
                entity.HasOne(e => e.DeliveryUser)
                    .WithMany()
                    .HasForeignKey(e => e.DeliveryUserId)
                    .OnDelete(DeleteBehavior.Cascade);
                
                entity.HasOne(e => e.DeliveryOrder)
                    .WithMany()
                    .HasForeignKey(e => e.DeliveryOrderId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
