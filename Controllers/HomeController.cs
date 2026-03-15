using Microsoft.AspNetCore.Mvc;
using RestaurantFoodOrderingDeliverUser.Data;
using RestaurantFoodOrderingDeliverUser.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantFoodOrderingDeliverUser.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly IDeliveryOrderRepository _deliveryOrderRepository;
        private readonly IDeliveryUserRepository _deliveryUserRepository;
        private readonly IDeliveryEarningRepository _deliveryEarningRepository;

        public HomeController(IAttendanceRepository attendanceRepository, IDeliveryOrderRepository deliveryOrderRepository, IDeliveryUserRepository deliveryUserRepository, IDeliveryEarningRepository deliveryEarningRepository)
        {
            _attendanceRepository = attendanceRepository;
            _deliveryOrderRepository = deliveryOrderRepository;
            _deliveryUserRepository = deliveryUserRepository;
            _deliveryEarningRepository = deliveryEarningRepository;
        }

        // GET: Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Check if user is logged in
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            int deliveryUserId = int.Parse(userId);
            
            // Get user details
            var user = await _deliveryUserRepository.GetByIdAsync(deliveryUserId);
            
            // Get IST time
            var istTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, 
                TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            var today = istTime.Date;
            
            // Get all orders for this user
            var allOrders = await _deliveryOrderRepository.GetAssignedOrdersAsync(deliveryUserId);
            
            // Calculate real stats
            var completedOrders = allOrders.Where(o => o.Status == "Delivered").ToList();
            var completedToday = completedOrders.Where(o => o.DeliveredTime.HasValue && o.DeliveredTime.Value.Date == today).Count();
            var pendingOrders = allOrders.Where(o => o.Status != "Delivered" && o.Status != "Cancelled").Count();
            
            ViewBag.TotalDeliveries = completedOrders.Count;
            ViewBag.CompletedToday = completedToday;
            ViewBag.PendingOrders = pendingOrders;
            ViewBag.Rating = user?.Rating ?? 0;
            ViewBag.TotalEarnings = user?.TotalEarnings ?? 0;
            ViewBag.DeliveryUserName = HttpContext.Session.GetString("DeliveryUserName");

            // Get active orders for display
            var activeOrders = allOrders.Where(o => o.Status != "Delivered" && o.Status != "Cancelled")
                .OrderBy(o => o.EstimatedDeliveryTime)
                .Take(10)
                .ToList();
            ViewBag.ActiveOrders = activeOrders;

            // Get pending earnings for display
            var pendingEarnings = await _deliveryEarningRepository.GetPendingEarningsAsync(deliveryUserId);
            ViewBag.PendingEarnings = pendingEarnings.Take(10).ToList();

            return View();
        }

        // GET: Profile
        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            int deliveryUserId = int.Parse(userId);
            var user = await _deliveryUserRepository.GetByIdAsync(deliveryUserId);
            
            if (user == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Get actual completed deliveries count
            var deliveredOrders = await _deliveryOrderRepository.GetDeliveredOrdersAsync(deliveryUserId, 10000);
            var completedDeliveries = deliveredOrders.Count;
            
            // Get actual total earnings
            var totalEarnings = await _deliveryEarningRepository.GetTotalEarningsAsync(deliveryUserId);

            // Update model with real values
            user.TotalDeliveries = completedDeliveries;
            user.TotalEarnings = totalEarnings;

            return View(user);
        }

        // POST: UpdateProfile
        [HttpPost]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateModel model)
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Not authenticated" });
            }

            int deliveryUserId = int.Parse(userId);
            var user = await _deliveryUserRepository.GetByIdAsync(deliveryUserId);
            
            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            try
            {
                // Update basic info
                if (!string.IsNullOrEmpty(model.FullName)) user.FullName = model.FullName;
                if (!string.IsNullOrEmpty(model.Email)) user.Email = model.Email;
                if (!string.IsNullOrEmpty(model.Phone)) user.Phone = model.Phone;
                
                // Update personal info
                if (model.DateOfBirth.HasValue) user.DateOfBirth = model.DateOfBirth;
                if (!string.IsNullOrEmpty(model.Gender)) user.Gender = model.Gender;
                if (!string.IsNullOrEmpty(model.FatherName)) user.FatherName = model.FatherName;
                if (!string.IsNullOrEmpty(model.AadharNumber)) user.AadharNumber = model.AadharNumber;
                if (!string.IsNullOrEmpty(model.PANNumber)) user.PANNumber = model.PANNumber;
                if (!string.IsNullOrEmpty(model.PermanentAddress)) user.PermanentAddress = model.PermanentAddress;
                if (!string.IsNullOrEmpty(model.CurrentAddress)) user.CurrentAddress = model.CurrentAddress;
                if (!string.IsNullOrEmpty(model.City)) user.City = model.City;
                if (!string.IsNullOrEmpty(model.State)) user.State = model.State;
                if (!string.IsNullOrEmpty(model.Pincode)) user.Pincode = model.Pincode;
                
                // Update medical info
                if (!string.IsNullOrEmpty(model.BloodGroup)) user.BloodGroup = model.BloodGroup;
                if (!string.IsNullOrEmpty(model.Height)) user.Height = model.Height;
                if (!string.IsNullOrEmpty(model.Weight)) user.Weight = model.Weight;
                if (!string.IsNullOrEmpty(model.Vision)) user.Vision = model.Vision;
                if (!string.IsNullOrEmpty(model.KnownAllergies)) user.KnownAllergies = model.KnownAllergies;
                if (!string.IsNullOrEmpty(model.ChronicConditions)) user.ChronicConditions = model.ChronicConditions;
                if (model.LastHealthCheckup.HasValue) user.LastHealthCheckup = model.LastHealthCheckup;
                if (model.MedicalInsuranceExpiry.HasValue) user.MedicalInsuranceExpiry = model.MedicalInsuranceExpiry;
                
                // Update vehicle info
                if (!string.IsNullOrEmpty(model.VehicleType)) user.VehicleType = model.VehicleType;
                if (!string.IsNullOrEmpty(model.VehicleModel)) user.VehicleModel = model.VehicleModel;
                if (!string.IsNullOrEmpty(model.VehicleNumber)) user.VehicleNumber = model.VehicleNumber;
                if (!string.IsNullOrEmpty(model.VehicleColor)) user.VehicleColor = model.VehicleColor;
                if (model.VehicleManufacturingYear.HasValue) user.VehicleManufacturingYear = model.VehicleManufacturingYear;
                if (model.VehicleInsuranceExpiry.HasValue) user.VehicleInsuranceExpiry = model.VehicleInsuranceExpiry;
                if (model.PUCExpiry.HasValue) user.PUCExpiry = model.PUCExpiry;
                if (model.FitnessCertificateExpiry.HasValue) user.FitnessCertificateExpiry = model.FitnessCertificateExpiry;
                
                // Update documents
                if (!string.IsNullOrEmpty(model.DrivingLicenseNumber)) user.DrivingLicenseNumber = model.DrivingLicenseNumber;
                if (model.DrivingLicenseExpiry.HasValue) user.DrivingLicenseExpiry = model.DrivingLicenseExpiry;
                if (!string.IsNullOrEmpty(model.VehicleInsuranceNumber)) user.VehicleInsuranceNumber = model.VehicleInsuranceNumber;
                if (!string.IsNullOrEmpty(model.PUCNumber)) user.PUCNumber = model.PUCNumber;
                if (!string.IsNullOrEmpty(model.MedicalInsuranceNumber)) user.MedicalInsuranceNumber = model.MedicalInsuranceNumber;
                
                // Update bank details
                if (!string.IsNullOrEmpty(model.BankAccountHolderName)) user.BankAccountHolderName = model.BankAccountHolderName;
                if (!string.IsNullOrEmpty(model.BankName)) user.BankName = model.BankName;
                if (!string.IsNullOrEmpty(model.BankAccountNumber)) user.BankAccountNumber = model.BankAccountNumber;
                if (!string.IsNullOrEmpty(model.BankIFSCCode)) user.BankIFSCCode = model.BankIFSCCode;
                if (!string.IsNullOrEmpty(model.BankBranch)) user.BankBranch = model.BankBranch;
                if (!string.IsNullOrEmpty(model.UPIID)) user.UPIID = model.UPIID;
                
                // Update emergency contact
                if (!string.IsNullOrEmpty(model.EmergencyContactName)) user.EmergencyContactName = model.EmergencyContactName;
                if (!string.IsNullOrEmpty(model.EmergencyContactPhone)) user.EmergencyContactPhone = model.EmergencyContactPhone;
                if (!string.IsNullOrEmpty(model.EmergencyContactRelation)) user.EmergencyContactRelation = model.EmergencyContactRelation;
                if (!string.IsNullOrEmpty(model.SecondaryContactName)) user.SecondaryContactName = model.SecondaryContactName;
                if (!string.IsNullOrEmpty(model.SecondaryContactPhone)) user.SecondaryContactPhone = model.SecondaryContactPhone;
                if (!string.IsNullOrEmpty(model.SecondaryContactRelation)) user.SecondaryContactRelation = model.SecondaryContactRelation;
                if (!string.IsNullOrEmpty(model.EmergencyAddress)) user.EmergencyAddress = model.EmergencyAddress;

                await _deliveryUserRepository.UpdateAsync(user);
                
                return Json(new { success = true, message = "Profile updated successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Earnings
        public async Task<IActionResult> Earnings(int? month, int? year)
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            int deliveryUserId = int.Parse(userId);
            
            // Use IST timezone
            var istTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, 
                TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));
            
            int currentMonth = month ?? istTime.Month;
            int currentYear = year ?? istTime.Year;
            
            // Get all earnings for the selected month
            var allEarnings = await _deliveryEarningRepository.GetUserEarningsForMonthAsync(deliveryUserId, currentMonth, currentYear);
            
            // Separate paid and pending earnings
            var paidEarnings = allEarnings.Where(e => e.PaymentStatus == "Paid").ToList();
            var pendingEarnings = allEarnings.Where(e => e.PaymentStatus == "Pending").ToList();
            
            // Calculate monthly total (only paid earnings)
            decimal monthlyTotal = paidEarnings.Sum(e => e.TotalEarning);
            
            // Calculate total earnings (all time, only paid)
            var totalEarnings = await _deliveryEarningRepository.GetTotalEarningsAsync(deliveryUserId);
            
            // Calculate pending amount (for selected month)
            decimal pendingAmount = pendingEarnings.Sum(e => e.TotalEarning);
            
            // Calculate breakdown from PAID earnings only
            decimal deliveryFees = paidEarnings.Sum(e => e.DeliveryFee);
            decimal tips = paidEarnings.Sum(e => e.TipAmount);
            decimal bonuses = paidEarnings.Sum(e => e.Bonus);
            decimal incentives = paidEarnings.Sum(e => e.Incentive);
            decimal deductions = paidEarnings.Sum(e => e.Deduction);
            
            ViewBag.CurrentMonth = currentMonth;
            ViewBag.CurrentYear = currentYear;
            ViewBag.MonthName = new DateTime(currentYear, currentMonth, 1).ToString("MMMM yyyy");
            ViewBag.MonthlyTotal = monthlyTotal;
            ViewBag.TotalEarnings = totalEarnings;
            ViewBag.PendingAmount = pendingAmount;
            ViewBag.DeliveryFees = deliveryFees;
            ViewBag.Tips = tips;
            ViewBag.Bonuses = bonuses;
            ViewBag.Incentives = incentives;
            ViewBag.Deductions = deductions;
            
            return View(allEarnings);
        }

        // GET: Attendance
        public async Task<IActionResult> Attendance()
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            int deliveryUserId = int.Parse(userId);
            
            // Get attendance records from database
            var attendanceList = await _attendanceRepository.GetUserAttendanceAsync(deliveryUserId, 30);

            return View(attendanceList);
        }

        // API: Get Today's Attendance
        [HttpGet]
        public async Task<IActionResult> GetTodayAttendance()
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Not logged in" });
            }

            int deliveryUserId = int.Parse(userId);
            var attendance = await _attendanceRepository.GetTodayAttendanceAsync(deliveryUserId);
            
            if (attendance == null)
            {
                return Json(new { 
                    success = true, 
                    data = (object?)null 
                });
            }

            return Json(new { 
                success = true, 
                data = new {
                    attendance.AttendanceId,
                    attendance.CheckInTime,
                    attendance.CheckOutTime,
                    attendance.IntermediateStartTime,
                    attendance.IntermediateEndTime,
                    attendance.InTimeReason,
                    attendance.OutTimeReason,
                    attendance.IntermediateStartReason,
                    attendance.IntermediateEndReason,
                    attendance.Status
                }
            });
        }

        // API: Record In Time
        [HttpPost]
        public async Task<IActionResult> RecordInTime([FromBody] AttendanceTimeRequest request)
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Not logged in" });
            }

            try
            {
                int deliveryUserId = int.Parse(userId);
                await _attendanceRepository.UpdateInTimeAsync(deliveryUserId, request.Time, request.Reason);
                return Json(new { success = true, message = "In Time recorded successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Record Intermediate Start
        [HttpPost]
        public async Task<IActionResult> RecordIntermediateStart([FromBody] AttendanceTimeRequest request)
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Not logged in" });
            }

            try
            {
                int deliveryUserId = int.Parse(userId);
                await _attendanceRepository.UpdateIntermediateStartAsync(deliveryUserId, request.Time, request.Reason);
                return Json(new { success = true, message = "Intermediate Start recorded successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Record Intermediate End
        [HttpPost]
        public async Task<IActionResult> RecordIntermediateEnd([FromBody] AttendanceTimeRequest request)
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Not logged in" });
            }

            try
            {
                int deliveryUserId = int.Parse(userId);
                await _attendanceRepository.UpdateIntermediateEndAsync(deliveryUserId, request.Time, request.Reason);
                return Json(new { success = true, message = "Intermediate End recorded successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API: Record Out Time
        [HttpPost]
        public async Task<IActionResult> RecordOutTime([FromBody] AttendanceTimeRequest request)
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Not logged in" });
            }

            try
            {
                int deliveryUserId = int.Parse(userId);
                await _attendanceRepository.UpdateOutTimeAsync(deliveryUserId, request.Time, request.Reason);
                return Json(new { success = true, message = "Out Time recorded successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Apply Leaves
        public IActionResult ApplyLeaves()
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        // POST: Apply Leaves
        [HttpPost]
        public IActionResult ApplyLeaves(DateTime startDate, DateTime endDate, string leaveType, string reason)
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            // TODO: Save leave request to database
            ViewBag.SuccessMessage = "Leave request submitted successfully!";
            return View();
        }

        // GET: View Leaves
        public IActionResult ViewLeaves()
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            // Mock leave data
            var leaveList = new List<Leave>
            {
                new Leave 
                { 
                    LeaveId = 1, 
                    StartDate = DateTime.Now.AddDays(5), 
                    EndDate = DateTime.Now.AddDays(7), 
                    LeaveType = "Casual",
                    Reason = "Personal work",
                    Status = "Pending",
                    AppliedDate = DateTime.Now
                },
                new Leave 
                { 
                    LeaveId = 2, 
                    StartDate = DateTime.Now.AddDays(-10), 
                    EndDate = DateTime.Now.AddDays(-8), 
                    LeaveType = "Sick",
                    Reason = "Medical checkup",
                    Status = "Approved",
                    ApprovedBy = "Admin",
                    ApprovedDate = DateTime.Now.AddDays(-9)
                }
            };

            return View(leaveList);
        }

        // GET: Calendar
        public async Task<IActionResult> Calendar(int? month, int? year)
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            int deliveryUserId = int.Parse(userId);

            // Default to current month and year
            int currentMonth = month ?? DateTime.Now.Month;
            int currentYear = year ?? DateTime.Now.Year;

            ViewBag.CurrentMonth = currentMonth;
            ViewBag.CurrentYear = currentYear;
            ViewBag.MonthName = new DateTime(currentYear, currentMonth, 1).ToString("MMMM yyyy");

            // Get first and last day of month
            var firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);

            // Calculate previous and next month
            var prevMonth = firstDayOfMonth.AddMonths(-1);
            var nextMonth = firstDayOfMonth.AddMonths(1);
            ViewBag.PrevMonth = prevMonth.Month;
            ViewBag.PrevYear = prevMonth.Year;
            ViewBag.NextMonth = nextMonth.Month;
            ViewBag.NextYear = nextMonth.Year;

            // Get attendance records from database for this month
            var attendanceRecords = await _attendanceRepository.GetMonthlyAttendanceAsync(deliveryUserId, currentMonth, currentYear);
            var attendanceDict = attendanceRecords.ToDictionary(a => a.AttendanceDate.Day);

            // Build calendar data from database
            var calendarData = new Dictionary<int, CalendarDay>();

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(currentYear, currentMonth, day);
                var dayOfWeek = date.DayOfWeek;
                
                var calendarDay = new CalendarDay
                {
                    Day = day,
                    Date = date
                };

                // Sundays are off
                if (dayOfWeek == DayOfWeek.Sunday)
                {
                    calendarDay.Status = "Off";
                    calendarDay.BackgroundColor = "#f0f0f0";
                }
                // Check if we have attendance record for this day
                else if (attendanceDict.ContainsKey(day))
                {
                    var record = attendanceDict[day];
                    calendarDay.Status = record.Status ?? "Present";
                    
                    // Calculate work hours if both in and out times exist
                    if (!string.IsNullOrEmpty(record.CheckInTime) && !string.IsNullOrEmpty(record.CheckOutTime))
                    {
                        calendarDay.WorkHours = CalculateWorkHours(record.CheckInTime, record.CheckOutTime, 
                            record.IntermediateStartTime, record.IntermediateEndTime);
                    }
                    else if (!string.IsNullOrEmpty(record.CheckInTime))
                    {
                        calendarDay.WorkHours = "In Progress";
                    }
                    
                    // Set background color based on status
                    calendarDay.BackgroundColor = record.Status switch
                    {
                        "Late" => "#FFD700",      // Yellow
                        "Absent" => "#FF6B6B",    // Red
                        "HalfDay" => "#FFB6C1",   // Light Pink
                        "Leave" => "#999999",     // Gray
                        _ => "#90EE90"            // Green for Present
                    };
                    
                    // Set reasons
                    calendarDay.LeaveReason = record.InTimeReason ?? record.OutTimeReason;
                    calendarDay.LeaveReason2 = record.IntermediateStartReason ?? record.IntermediateEndReason;
                }
                // Future dates or no record
                else if (date > DateTime.Now.Date)
                {
                    calendarDay.Status = "";
                    calendarDay.BackgroundColor = "#ffffff";
                }
                else
                {
                    // Past date with no record - Absent
                    calendarDay.Status = "Absent";
                    calendarDay.BackgroundColor = "#FF6B6B";
                }

                calendarData[day] = calendarDay;
            }

            // Calculate starting day of week (0 = Sunday)
            ViewBag.StartDayOfWeek = (int)firstDayOfMonth.DayOfWeek;
            ViewBag.DaysInMonth = daysInMonth;

            return View(calendarData);
        }

        // Helper method to calculate work hours
        private string CalculateWorkHours(string inTime, string outTime, string? breakStart, string? breakEnd)
        {
            try
            {
                // Parse times (format: "09:00 AM")
                if (DateTime.TryParse(inTime, out var inDateTime) && DateTime.TryParse(outTime, out var outDateTime))
                {
                    var totalMinutes = (outDateTime - inDateTime).TotalMinutes;
                    
                    // Subtract break time if available
                    if (!string.IsNullOrEmpty(breakStart) && !string.IsNullOrEmpty(breakEnd))
                    {
                        if (DateTime.TryParse(breakStart, out var breakStartTime) && DateTime.TryParse(breakEnd, out var breakEndTime))
                        {
                            totalMinutes -= (breakEndTime - breakStartTime).TotalMinutes;
                        }
                    }
                    
                    var hours = (int)(totalMinutes / 60);
                    var minutes = (int)(totalMinutes % 60);
                    return $"{hours:D2}:{minutes:D2}:00";
                }
            }
            catch
            {
                // Ignore parsing errors
            }
            return "00:00:00";
        }

        // GET: DeliveryOrders - Delivery Partner's assigned orders
        public async Task<IActionResult> DeliveryOrders()
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            int deliveryUserId = int.Parse(userId);

            var orders = await _deliveryOrderRepository.GetAssignedOrdersAsync(deliveryUserId);
            ViewBag.TodayDeliveries = await _deliveryOrderRepository.GetTodayDeliveryCountAsync(deliveryUserId);
            ViewBag.TodayEarnings = await _deliveryOrderRepository.GetTodayEarningsAsync(deliveryUserId);

            return View(orders);
        }

        // POST: Pick up order
        [HttpPost]
        public async Task<IActionResult> PickupOrder([FromBody] OrderStatusRequest request)
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Not logged in" });
            }

            try
            {
                await _deliveryOrderRepository.MarkAsPickedUpAsync(request.OrderId);
                return Json(new { success = true, message = "Order picked up successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: Deliver order
        [HttpPost]
        public async Task<IActionResult> DeliverOrder([FromBody] OrderStatusRequest request)
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Not logged in" });
            }

            try
            {
                await _deliveryOrderRepository.MarkAsDeliveredAsync(request.OrderId);
                return Json(new { success = true, message = "Order delivered successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: Rules
        [HttpGet]
        public IActionResult Rules()
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        // GET: About
        [HttpGet]
        public IActionResult About()
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        // GET: Contact
        [HttpGet]
        public IActionResult Contact()
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        // GET: Privacy Policy
        [HttpGet]
        public IActionResult PrivacyPolicy()
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        // GET: Terms of Service
        [HttpGet]
        public IActionResult TermsOfService()
        {
            var userId = HttpContext.Session.GetString("DeliveryUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }
    }

    // Request model for attendance time API
    public class AttendanceTimeRequest
    {
        public string? Time { get; set; }
        public string? Reason { get; set; }
    }

    // Request model for order status updates
    public class OrderStatusRequest
    {
        public int OrderId { get; set; }
    }

    // Calendar Day Model
    public class CalendarDay
    {
        public int Day { get; set; }
        public DateTime Date { get; set; }
        public string? WorkHours { get; set; }
        public string? Status { get; set; }
        public string? BackgroundColor { get; set; }
        public string? LeaveReason { get; set; }
        public string? LeaveReason2 { get; set; }
    }
}
