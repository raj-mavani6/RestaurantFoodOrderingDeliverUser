-- =============================================
-- TABLE 13: DeliveryUsers (Delivery Admin + Delivery User Panel)
-- =============================================
CREATE TABLE IF NOT EXISTS `DeliveryUsers` (
    `DeliveryUserId` INT NOT NULL AUTO_INCREMENT,
    `FullName` VARCHAR(100) NOT NULL,
    `Email` VARCHAR(100) NOT NULL,
    `Phone` VARCHAR(15) NOT NULL,
    `Password` VARCHAR(255) NOT NULL,
    `PermanentAddress` VARCHAR(500) NULL,
    `CurrentAddress` VARCHAR(500) NULL,
    `City` VARCHAR(100) NULL,
    `State` VARCHAR(100) NULL,
    `Pincode` VARCHAR(10) NULL,
    `VehicleType` VARCHAR(50) NULL,
    `VehicleNumber` VARCHAR(20) NULL,
    `Status` VARCHAR(50) NOT NULL DEFAULT 'Active',
    `Rating` DECIMAL(3,1) NOT NULL DEFAULT 0.0,
    `TotalDeliveries` INT NOT NULL DEFAULT 0,
    `TotalEarnings` DECIMAL(12,2) NOT NULL DEFAULT 0.00,
    `JoinDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
    PRIMARY KEY (`DeliveryUserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- TABLE 14: Attendance (Delivery Admin + Delivery User Panel)
-- =============================================
CREATE TABLE IF NOT EXISTS `Attendance` (
    `AttendanceId` INT NOT NULL AUTO_INCREMENT,
    `DeliveryUserId` INT NOT NULL,
    `AttendanceDate` DATETIME NOT NULL,
    `CheckInTime` VARCHAR(20) NULL,
    `CheckOutTime` VARCHAR(20) NULL,
    `IntermediateStartTime` VARCHAR(20) NULL,
    `IntermediateEndTime` VARCHAR(20) NULL,
    `InTimeReason` VARCHAR(500) NULL,
    `OutTimeReason` VARCHAR(500) NULL,
    `IntermediateStartReason` VARCHAR(500) NULL,
    `IntermediateEndReason` VARCHAR(500) NULL,
    `Status` VARCHAR(50) NOT NULL DEFAULT 'Present',
    `Notes` VARCHAR(500) NULL,
    `OrdersCompleted` INT NOT NULL DEFAULT 0,
    `DistanceCovered` DECIMAL(8,2) NOT NULL DEFAULT 0.00,
    PRIMARY KEY (`AttendanceId`),
    CONSTRAINT `FK_Attendance_DeliveryUsers` FOREIGN KEY (`DeliveryUserId`)
        REFERENCES `DeliveryUsers` (`DeliveryUserId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- TABLE 15: Leaves (Delivery Admin + Delivery User Panel)
-- =============================================
CREATE TABLE IF NOT EXISTS `Leaves` (
    `LeaveId` INT NOT NULL AUTO_INCREMENT,
    `DeliveryUserId` INT NOT NULL,
    `StartDate` DATETIME NOT NULL,
    `EndDate` DATETIME NOT NULL,
    `LeaveType` VARCHAR(50) NOT NULL,
    `Reason` VARCHAR(500) NOT NULL,
    `Status` VARCHAR(50) NOT NULL DEFAULT 'Pending',
    `ApprovedBy` VARCHAR(100) NULL,
    `AdminNotes` VARCHAR(500) NULL,
    `AppliedDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (`LeaveId`),
    CONSTRAINT `FK_Leaves_DeliveryUsers` FOREIGN KEY (`DeliveryUserId`)
        REFERENCES `DeliveryUsers` (`DeliveryUserId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- TABLE 16: DeliveryOrders (Delivery Admin + Delivery User Panel)
-- =============================================
CREATE TABLE IF NOT EXISTS `DeliveryOrders` (
    `DeliveryOrderId` INT NOT NULL AUTO_INCREMENT,
    `OrderId` INT NULL,
    `DeliveryUserId` INT NOT NULL,
    `CustomerName` VARCHAR(100) NULL,
    `CustomerPhone` VARCHAR(15) NULL,
    `DeliveryAddress` VARCHAR(500) NULL,
    `Status` VARCHAR(50) NOT NULL DEFAULT 'Assigned',
    `TotalAmount` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    `DeliveryFee` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    `TipAmount` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    `Distance` DECIMAL(5,2) NOT NULL DEFAULT 0.00,
    `PaymentMethod` VARCHAR(50) NULL,
    `PaymentStatus` VARCHAR(50) NULL,
    `OrderTime` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `PickupTime` DATETIME NULL,
    `DeliveryTime` DATETIME NULL,
    `Notes` VARCHAR(500) NULL,
    PRIMARY KEY (`DeliveryOrderId`),
    CONSTRAINT `FK_DeliveryOrders_DeliveryUsers` FOREIGN KEY (`DeliveryUserId`)
        REFERENCES `DeliveryUsers` (`DeliveryUserId`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- TABLE 17: DeliveryEarning (Delivery Admin + Delivery User Panel)
-- =============================================
CREATE TABLE IF NOT EXISTS `DeliveryEarning` (
    `EarningId` INT NOT NULL AUTO_INCREMENT,
    `DeliveryUserId` INT NOT NULL,
    `DeliveryOrderId` INT NULL,
    `DeliveryFee` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    `TipAmount` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    `Bonus` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    `Incentive` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    `Deduction` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    `TotalEarning` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
    `EarningType` VARCHAR(50) NULL,
    `Description` VARCHAR(200) NULL,
    `EarningDate` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `PaymentStatus` VARCHAR(50) NULL DEFAULT 'Pending',
    `PaymentMethod` VARCHAR(50) NULL,
    `TransactionId` VARCHAR(100) NULL,
    PRIMARY KEY (`EarningId`),
    CONSTRAINT `FK_DeliveryEarning_DeliveryUsers` FOREIGN KEY (`DeliveryUserId`)
        REFERENCES `DeliveryUsers` (`DeliveryUserId`) ON DELETE CASCADE,
    CONSTRAINT `FK_DeliveryEarning_DeliveryOrders` FOREIGN KEY (`DeliveryOrderId`)
        REFERENCES `DeliveryOrders` (`DeliveryOrderId`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- =============================================
-- Add FK: Orders -> DeliveryUsers (after DeliveryUsers table is created)
-- =============================================
ALTER TABLE `Orders`
ADD CONSTRAINT `FK_Orders_DeliveryUsers` FOREIGN KEY (`DeliveryUserId`)
    REFERENCES `DeliveryUsers` (`DeliveryUserId`) ON DELETE SET NULL;

-- =============================================
-- INSERT: DeliveryUsers
-- =============================================
INSERT INTO `DeliveryUsers` (`FullName`, `Email`, `Phone`, `Password`, `PermanentAddress`, `CurrentAddress`, `City`, `State`, `Pincode`, `VehicleType`, `VehicleNumber`, `Status`, `Rating`, `TotalDeliveries`, `TotalEarnings`, `IsActive`) VALUES
('Vijay Kumar', 'vijay@delivery.com', '9988776601', 'delivery123', '101 Maninagar, Ahmedabad', '101 Maninagar, Ahmedabad', 'Ahmedabad', 'Gujarat', '380008', 'Bike', 'GJ01AB1234', 'Active', 4.5, 120, 24000.00, 1),
('Suresh Patel', 'suresh@delivery.com', '9988776602', 'delivery123', '202 Vastrapur, Ahmedabad', '202 Vastrapur, Ahmedabad', 'Ahmedabad', 'Gujarat', '380015', 'Scooter', 'GJ01CD5678', 'Active', 4.2, 85, 17000.00, 1),
('Ravi Joshi', 'ravi@delivery.com', '9988776603', 'delivery123', '303 Bopal, Ahmedabad', '303 Bopal, Ahmedabad', 'Ahmedabad', 'Gujarat', '380058', 'Bike', 'GJ01EF9012', 'Active', 4.8, 200, 40000.00, 1),
('Mahesh Shah', 'mahesh@delivery.com', '9988776604', 'delivery123', '404 Gota, Ahmedabad', '404 Gota, Ahmedabad', 'Ahmedabad', 'Gujarat', '382481', 'Bike', 'GJ01GH3456', 'Inactive', 3.9, 45, 9000.00, 0),
('Dinesh Thakor', 'dinesh@delivery.com', '9988776605', 'delivery123', '505 Chandkheda, Ahmedabad', '505 Chandkheda, Ahmedabad', 'Ahmedabad', 'Gujarat', '382424', 'Scooter', 'GJ01IJ7890', 'Active', 4.0, 60, 12000.00, 1);

-- =============================================
-- INSERT: Attendance
-- =============================================
INSERT INTO `Attendance` (`DeliveryUserId`, `AttendanceDate`, `CheckInTime`, `CheckOutTime`, `Status`, `OrdersCompleted`, `DistanceCovered`) VALUES
(1, DATE_SUB(CURDATE(), INTERVAL 2 DAY), '09:00 AM', '06:00 PM', 'Present', 8, 45.50),
(1, DATE_SUB(CURDATE(), INTERVAL 1 DAY), '09:15 AM', '06:30 PM', 'Present', 10, 52.30),
(1, CURDATE(), '09:00 AM', NULL, 'Present', 3, 15.00),
(2, DATE_SUB(CURDATE(), INTERVAL 2 DAY), '10:00 AM', '07:00 PM', 'Present', 6, 38.20),
(2, DATE_SUB(CURDATE(), INTERVAL 1 DAY), NULL, NULL, 'Absent', 0, 0.00),
(2, CURDATE(), '09:30 AM', NULL, 'Present', 2, 10.50),
(3, DATE_SUB(CURDATE(), INTERVAL 2 DAY), '08:30 AM', '05:30 PM', 'Present', 12, 65.00),
(3, DATE_SUB(CURDATE(), INTERVAL 1 DAY), '08:45 AM', '06:00 PM', 'Present', 11, 58.70),
(3, CURDATE(), '08:30 AM', NULL, 'Present', 5, 28.40),
(5, CURDATE(), '09:00 AM', NULL, 'Present', 4, 20.00);

-- =============================================
-- INSERT: Leaves
-- =============================================
INSERT INTO `Leaves` (`DeliveryUserId`, `StartDate`, `EndDate`, `LeaveType`, `Reason`, `Status`, `ApprovedBy`, `AdminNotes`) VALUES
(1, DATE_ADD(CURDATE(), INTERVAL 3 DAY), DATE_ADD(CURDATE(), INTERVAL 4 DAY), 'Sick Leave', 'Not feeling well, need rest.', 'Approved', 'Admin', 'Take care, approved.'),
(2, DATE_ADD(CURDATE(), INTERVAL 5 DAY), DATE_ADD(CURDATE(), INTERVAL 7 DAY), 'Personal Leave', 'Family function to attend.', 'Pending', NULL, NULL),
(4, DATE_SUB(CURDATE(), INTERVAL 10 DAY), DATE_SUB(CURDATE(), INTERVAL 8 DAY), 'Sick Leave', 'Had fever.', 'Approved', 'Admin', 'Get well soon.'),
(3, DATE_ADD(CURDATE(), INTERVAL 15 DAY), DATE_ADD(CURDATE(), INTERVAL 16 DAY), 'Casual Leave', 'Personal work.', 'Pending', NULL, NULL),
(5, DATE_SUB(CURDATE(), INTERVAL 3 DAY), DATE_SUB(CURDATE(), INTERVAL 2 DAY), 'Emergency Leave', 'Urgent family emergency.', 'Approved', 'Admin', 'Approved immediately.');

-- =============================================
-- INSERT: DeliveryOrders
-- =============================================
INSERT INTO `DeliveryOrders` (`OrderId`, `DeliveryUserId`, `CustomerName`, `CustomerPhone`, `DeliveryAddress`, `Status`, `TotalAmount`, `DeliveryFee`, `TipAmount`, `Distance`, `PaymentMethod`, `PaymentStatus`, `OrderTime`, `PickupTime`, `DeliveryTime`) VALUES
(1, 1, 'Rahul Sharma', '9876543210', '123 MG Road, Ahmedabad', 'Delivered', 448.00, 40.00, 20.00, 5.50, 'Online', 'Paid', DATE_SUB(NOW(), INTERVAL 5 DAY), DATE_SUB(NOW(), INTERVAL 5 DAY) + INTERVAL 20 MINUTE, DATE_SUB(NOW(), INTERVAL 5 DAY) + INTERVAL 50 MINUTE),
(2, 3, 'Priya Patel', '9876543211', '456 SG Highway, Ahmedabad', 'Delivered', 349.00, 35.00, 15.00, 4.20, 'COD', 'Paid', DATE_SUB(NOW(), INTERVAL 4 DAY), DATE_SUB(NOW(), INTERVAL 4 DAY) + INTERVAL 15 MINUTE, DATE_SUB(NOW(), INTERVAL 4 DAY) + INTERVAL 45 MINUTE),
(5, 2, 'Neha Desai', '9876543213', '321 Navrangpura, Ahmedabad', 'Out for Delivery', 378.00, 30.00, 0.00, 3.80, 'Online', 'Paid', DATE_SUB(NOW(), INTERVAL 2 HOUR), DATE_SUB(NOW(), INTERVAL 90 MINUTE), NULL),
(3, 1, 'Amit Singh', '9876543212', '789 CG Road, Ahmedabad', 'Assigned', 547.00, 45.00, 0.00, 6.10, 'Online', 'Paid', DATE_SUB(NOW(), INTERVAL 1 DAY), NULL, NULL);

-- =============================================
-- INSERT: DeliveryEarning
-- =============================================
INSERT INTO `DeliveryEarning` (`DeliveryUserId`, `DeliveryOrderId`, `DeliveryFee`, `TipAmount`, `Bonus`, `Incentive`, `Deduction`, `TotalEarning`, `EarningType`, `Description`, `PaymentStatus`, `PaymentMethod`, `TransactionId`) VALUES
(1, 1, 40.00, 20.00, 10.00, 5.00, 0.00, 75.00, 'Delivery', 'Order #1 delivery completed', 'Paid', 'Bank Transfer', 'TXN001'),
(3, 2, 35.00, 15.00, 0.00, 5.00, 0.00, 55.00, 'Delivery', 'Order #2 delivery completed', 'Paid', 'Bank Transfer', 'TXN002'),
(2, 3, 30.00, 0.00, 0.00, 0.00, 0.00, 30.00, 'Delivery', 'Order #5 in transit', 'Pending', NULL, NULL),
(1, 4, 45.00, 0.00, 0.00, 0.00, 0.00, 45.00, 'Delivery', 'Order #3 assigned', 'Pending', NULL, NULL),
(1, NULL, 0.00, 0.00, 100.00, 0.00, 0.00, 100.00, 'Bonus', 'Weekly performance bonus', 'Paid', 'Bank Transfer', 'TXN003'),
(3, NULL, 0.00, 0.00, 150.00, 50.00, 0.00, 200.00, 'Bonus', 'Monthly top performer reward', 'Paid', 'Bank Transfer', 'TXN004');
