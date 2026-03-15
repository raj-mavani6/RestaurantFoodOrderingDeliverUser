// ================================================
// Restaurant Food Ordering - Delivery User Panel
// JavaScript Functions
// ================================================

// ================================================
// 1. Utility Functions
// ================================================

// Show toast notification
function showToast(message, type = 'info') {
    const toastHtml = `
        <div class="toast-notification toast-${type}">
            <i class="fas fa-${getIconByType(type)} me-2"></i>
            ${message}
        </div>
    `;

    const toastContainer = document.getElementById('toastContainer') || createToastContainer();
    toastContainer.insertAdjacentHTML('beforeend', toastHtml);

    const toast = toastContainer.lastElementChild;
    setTimeout(() => {
        toast.classList.add('show');
    }, 10);

    setTimeout(() => {
        toast.classList.remove('show');
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

function getIconByType(type) {
    const icons = {
        'success': 'check-circle',
        'error': 'exclamation-circle',
        'warning': 'exclamation-triangle',
        'info': 'info-circle'
    };
    return icons[type] || 'info-circle';
}

function createToastContainer() {
    const container = document.createElement('div');
    container.id = 'toastContainer';
    container.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 9999;
    `;
    document.body.appendChild(container);
    return container;
}

// ================================================
// 2. Dashboard Functions
// ================================================

// Initialize dashboard
function initializeDashboard() {
    loadDashboardData();
    setupRefreshInterval();
}

function loadDashboardData() {
    // TODO: Fetch data from server
    console.log('Loading dashboard data...');
}

function setupRefreshInterval() {
    // Refresh data every 5 minutes
    setInterval(() => {
        loadDashboardData();
    }, 5 * 60 * 1000);
}

// ================================================
// 3. Attendance Functions
// ================================================

// TimeSheet data storage
let timesheetData = {
    inTime: null,
    intermediateStart: null,
    intermediateEnd: null,
    outTime: null,
    reasons: {
        inTime: null,
        intermediateStart: null,
        intermediateEnd: null,
        outTime: null
    }
};

// Work hours configuration (Day Shift: 9:00 AM to 9:00 PM)
const workHoursConfig = {
    inTime: { hour: 9, minute: 0 },        // 9:00 AM
    outTime: { hour: 21, minute: 0 },      // 9:00 PM
    intermediateStart: { hour: 13, minute: 0 },  // 1:00 PM
    intermediateEnd: { hour: 14, minute: 0 }     // 2:00 PM
};

// Current action being processed (for popup callback)
let pendingAction = null;

// Indian Standard Time (IST) offset: UTC+5:30
const IST_OFFSET = 5.5 * 60 * 60 * 1000; // 5 hours 30 minutes in milliseconds

// Get current time in Indian Standard Time (IST)
function getIndianTime() {
    const now = new Date();
    // Get UTC time
    const utc = now.getTime() + (now.getTimezoneOffset() * 60 * 1000);
    // Add IST offset
    return new Date(utc + IST_OFFSET);
}

// Update current time display (in IST) - 12-hour format
function updateCurrentTime() {
    const now = getIndianTime();
    let hours = now.getHours();
    const isPM = hours >= 12;
    hours = hours % 12 || 12; // Convert to 12-hour format (12 instead of 0)
    const hoursStr = String(hours).padStart(2, '0');
    const minutes = String(now.getMinutes()).padStart(2, '0');
    const seconds = String(now.getSeconds()).padStart(2, '0');
    const ampm = isPM ? 'PM' : 'AM';

    const currentTimeElement = document.getElementById('currentTime');
    const ampmElement = document.getElementById('ampm');

    if (currentTimeElement) {
        currentTimeElement.textContent = `${hoursStr}:${minutes}:${seconds}`;
    }
    if (ampmElement) {
        ampmElement.textContent = ampm;
    }

    // Update day badge dynamically
    updateDayBadge();
}

// Update day badge based on IST
function updateDayBadge() {
    const now = getIndianTime();
    const days = ['SUN', 'MON', 'TUE', 'WED', 'THU', 'FRI', 'SAT'];
    const dayBadge = document.querySelector('.day-badge');
    if (dayBadge) {
        dayBadge.textContent = days[now.getDay()];
    }
}

// Helper: Check if current time is within day shift hours (9 AM to 9 PM) in IST
function isWithinDayShift() {
    const now = getIndianTime();
    const currentHour = now.getHours();
    // Day shift: 09:00 (9 AM) to 21:00 (9 PM)
    // Valid hours: 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20
    return currentHour >= 9 && currentHour < 21;
}

// Check if reason is needed for In Time (after 9:00 AM)
// User should clock in AT 9:00 AM, if they clock in AFTER 9:00 AM, need reason
function isLateInTime() {
    const now = getIndianTime();
    const currentHour = now.getHours();
    const currentMinute = now.getMinutes();

    // If before 9:00 AM - on time or early (no reason needed)
    if (currentHour < 9) {
        return false;
    }

    // Exactly at 9:00 AM - on time (no reason needed)
    if (currentHour === 9 && currentMinute === 0) {
        return false;
    }

    // After 9:00 AM - late (reason needed)
    if (currentHour > 9 || (currentHour === 9 && currentMinute > 0)) {
        return true;
    }

    return false;
}

// Check if reason is needed for Out Time (before 9:00 PM)
// User should clock out AT 9:00 PM, if they clock out BEFORE 9:00 PM, need reason
function isEarlyOutTime() {
    const now = getIndianTime();
    const currentHour = now.getHours();
    const currentMinute = now.getMinutes();

    // Before 9:00 PM (21:00) - early (reason needed)
    if (currentHour < 21) {
        return true;
    }

    // At or after 9:00 PM - on time (no reason needed)
    return false;
}

// Check if reason is needed for Intermediate Start (after 1:00 PM)
// Break starts at 1:00 PM, if they start break AFTER 1:00 PM, need reason
function isLateIntermediateStart() {
    const now = getIndianTime();
    const currentHour = now.getHours();
    const currentMinute = now.getMinutes();

    // Before 1:00 PM (13:00) - on time or early (no reason needed)
    if (currentHour < 13) {
        return false;
    }

    // Exactly at 1:00 PM - on time (no reason needed)
    if (currentHour === 13 && currentMinute === 0) {
        return false;
    }

    // After 1:00 PM - late (reason needed)
    if (currentHour > 13 || (currentHour === 13 && currentMinute > 0)) {
        return true;
    }

    return false;
}

// Check if reason is needed for Intermediate End (before 2:00 PM)
// Break ends at 2:00 PM, if they end break BEFORE 2:00 PM, need reason
function isEarlyIntermediateEnd() {
    const now = getIndianTime();
    const currentHour = now.getHours();
    const currentMinute = now.getMinutes();

    // Before 2:00 PM (14:00) - early (reason needed)
    if (currentHour < 14) {
        return true;
    }

    // At or after 2:00 PM - on time (no reason needed)
    return false;
}

// Show reason popup modal
function showReasonPopup(actionType, message) {
    pendingAction = actionType;

    const modal = document.getElementById('reasonModal');
    const messageElement = document.getElementById('reasonMessage');
    const reasonInput = document.getElementById('reasonInput');

    if (modal && messageElement && reasonInput) {
        messageElement.textContent = message;
        reasonInput.value = '';
        modal.style.display = 'flex';
        reasonInput.focus();
    }
}

// Close reason popup modal
function closeReasonPopup() {
    const modal = document.getElementById('reasonModal');
    if (modal) {
        modal.style.display = 'none';
    }
    pendingAction = null;
}

// Submit reason and proceed with action
function submitReason() {
    const reasonInput = document.getElementById('reasonInput');
    const reason = reasonInput ? reasonInput.value.trim() : '';

    if (!reason) {
        showToast('Please enter a reason to proceed', 'warning');
        return;
    }

    // Store the reason
    if (pendingAction) {
        timesheetData.reasons[pendingAction] = reason;
    }

    // Save the action before closing modal (closeReasonPopup sets pendingAction to null)
    const actionToExecute = pendingAction;

    // Close the modal
    const modal = document.getElementById('reasonModal');
    if (modal) {
        modal.style.display = 'none';
    }

    // Execute the pending action
    switch (actionToExecute) {
        case 'inTime':
            executeInTime();
            break;
        case 'intermediateStart':
            executeIntermediateStart();
            break;
        case 'intermediateEnd':
            executeIntermediateEnd();
            break;
        case 'outTime':
            executeOutTime();
            break;
    }

    pendingAction = null;
}

// Record In Time
function recordInTime() {
    if (isLateInTime()) {
        showReasonPopup('inTime', 'You are recording In Time after 9:00 AM. Please provide a reason:');
        return;
    }
    executeInTime();
}

function executeInTime() {
    const now = getIndianTime();
    const timeString = now.toLocaleTimeString('en-US', {
        hour: '2-digit',
        minute: '2-digit',
        hour12: true,
        timeZone: 'Asia/Kolkata'
    });

    // Save to database
    const reason = timesheetData.reasons.inTime || '';
    saveAttendanceToDatabase('RecordInTime', timeString, reason);

    timesheetData.inTime = now;
    document.getElementById('inTime').textContent = timeString;

    const reasonMsg = reason ? ` (Reason: ${reason})` : '';
    showToast(`✓ In Time recorded at ${timeString}${reasonMsg}`, 'success');
    saveToLocalStorage('timesheetData', timesheetData);
    calculateWorkedHours();
}

// Record Intermediate Start (Break Start)
function recordIntermediateStart() {
    if (isLateIntermediateStart()) {
        showReasonPopup('intermediateStart', 'You are recording Break Start after 1:00 PM. Please provide a reason:');
        return;
    }
    executeIntermediateStart();
}

function executeIntermediateStart() {
    const now = getIndianTime();
    const timeString = now.toLocaleTimeString('en-US', {
        hour: '2-digit',
        minute: '2-digit',
        hour12: true,
        timeZone: 'Asia/Kolkata'
    });

    // Save to database
    const reason = timesheetData.reasons.intermediateStart || '';
    saveAttendanceToDatabase('RecordIntermediateStart', timeString, reason);

    timesheetData.intermediateStart = now;
    document.getElementById('breakStart').textContent = timeString;

    const reasonMsg = reason ? ` (Reason: ${reason})` : '';
    showToast(`⏸ Break Start recorded at ${timeString}${reasonMsg}`, 'success');
    saveToLocalStorage('timesheetData', timesheetData);
    calculateWorkedHours();
}

// Record Intermediate End (Break End)
function recordIntermediateEnd() {
    if (isEarlyIntermediateEnd()) {
        showReasonPopup('intermediateEnd', 'You are recording Break End before 2:00 PM. Please provide a reason:');
        return;
    }
    executeIntermediateEnd();
}

function executeIntermediateEnd() {
    const now = getIndianTime();
    const timeString = now.toLocaleTimeString('en-US', {
        hour: '2-digit',
        minute: '2-digit',
        hour12: true,
        timeZone: 'Asia/Kolkata'
    });

    // Save to database
    const reason = timesheetData.reasons.intermediateEnd || '';
    saveAttendanceToDatabase('RecordIntermediateEnd', timeString, reason);

    timesheetData.intermediateEnd = now;
    document.getElementById('breakEnd').textContent = timeString;

    const reasonMsg = reason ? ` (Reason: ${reason})` : '';
    showToast(`▶ Break End recorded at ${timeString}${reasonMsg}`, 'success');
    saveToLocalStorage('timesheetData', timesheetData);
    calculateWorkedHours();
}

// Record Out Time
function recordOutTime() {
    if (isEarlyOutTime()) {
        showReasonPopup('outTime', 'You are recording Out Time before 9:00 PM. Please provide a reason:');
        return;
    }
    executeOutTime();
}

function executeOutTime() {
    const now = getIndianTime();
    const timeString = now.toLocaleTimeString('en-US', {
        hour: '2-digit',
        minute: '2-digit',
        hour12: true,
        timeZone: 'Asia/Kolkata'
    });

    // Save to database
    const reason = timesheetData.reasons.outTime || '';
    saveAttendanceToDatabase('RecordOutTime', timeString, reason);

    timesheetData.outTime = now;
    document.getElementById('outTime').textContent = timeString;

    const reasonMsg = reason ? ` (Reason: ${reason})` : '';
    showToast(`✓ Out Time recorded at ${timeString}${reasonMsg}`, 'success');
    saveToLocalStorage('timesheetData', timesheetData);
    calculateWorkedHours();
}

// Save attendance time to database via API
async function saveAttendanceToDatabase(action, time, reason) {
    try {
        console.log(`Saving to database: action=${action}, time=${time}, reason=${reason}`);

        const response = await fetch(`/Home/${action}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({ time: time, reason: reason })
        });

        const result = await response.json();
        console.log('Database response:', result);

        if (!result.success) {
            console.error('Error saving to database:', result.message);
            showToast(`⚠️ Database error: ${result.message}`, 'error');
        } else {
            console.log('Successfully saved to database');
        }
    } catch (error) {
        console.error('Error saving to database:', error);
        showToast(`⚠️ Failed to save to database: ${error.message}`, 'error');
    }
}

// Load today's attendance from database
async function loadTodayAttendance() {
    try {
        const response = await fetch('/Home/GetTodayAttendance');
        const result = await response.json();

        if (result.success && result.data) {
            const data = result.data;

            // Restore UI from database data
            if (data.checkInTime) {
                document.getElementById('inTime').textContent = data.checkInTime;
            }
            if (data.intermediateStartTime) {
                document.getElementById('breakStart').textContent = data.intermediateStartTime;
            }
            if (data.intermediateEndTime) {
                document.getElementById('breakEnd').textContent = data.intermediateEndTime;
            }
            if (data.checkOutTime) {
                document.getElementById('outTime').textContent = data.checkOutTime;
            }

            // Store reasons
            timesheetData.reasons.inTime = data.inTimeReason || null;
            timesheetData.reasons.intermediateStart = data.intermediateStartReason || null;
            timesheetData.reasons.intermediateEnd = data.intermediateEndReason || null;
            timesheetData.reasons.outTime = data.outTimeReason || null;
        }
    } catch (error) {
        console.error('Error loading attendance from database:', error);
        // Fall back to localStorage
        const savedData = getFromLocalStorage('timesheetData');
        if (savedData) {
            timesheetData = savedData;
        }
    }
}

// Calculate worked hours and break duration
function calculateWorkedHours() {
    if (!timesheetData.inTime || !timesheetData.outTime) {
        return;
    }

    // Calculate total time from in to out
    const totalMs = timesheetData.outTime - timesheetData.inTime;
    const totalHours = Math.floor(totalMs / (1000 * 60 * 60));
    const totalMinutes = Math.floor((totalMs % (1000 * 60 * 60)) / (1000 * 60));

    // Calculate break duration
    let breakMs = 0;
    if (timesheetData.intermediateStart && timesheetData.intermediateEnd) {
        breakMs = timesheetData.intermediateEnd - timesheetData.intermediateStart;
    }

    const breakHours = Math.floor(breakMs / (1000 * 60 * 60));
    const breakMinutes = Math.floor((breakMs % (1000 * 60 * 60)) / (1000 * 60));

    // Calculate actual worked hours
    const actualWorkedMs = totalMs - breakMs;
    const workedHours = Math.floor(actualWorkedMs / (1000 * 60 * 60));
    const workedMinutes = Math.floor((actualWorkedMs % (1000 * 60 * 60)) / (1000 * 60));

    // Update UI
    document.getElementById('workedHours').textContent = `${workedHours}h ${workedMinutes}m`;
    document.getElementById('breakDuration').textContent = `${breakHours}h ${breakMinutes}m`;
}

// Legacy functions for backward compatibility
function checkIn() {
    recordInTime();
}

function checkOut() {
    recordOutTime();
}

// ================================================
// 4. Leave Functions
// ================================================

// Calculate number of days between dates
// Calculate number of days between dates
function calculateLeaveDays() {
    const leaveType = document.getElementById('leaveType').value;
    const startDateInput = document.getElementById('startDate');
    const endDateInput = document.getElementById('endDate');
    const numberOfDays = document.getElementById('numberOfDays');

    if (leaveType === 'SingleDay' && startDateInput.value) {
        // For single day, end date = start date
        endDateInput.value = startDateInput.value;
        numberOfDays.value = 1;
        validateLeaveBalance(1);
    } else if (startDateInput && endDateInput && startDateInput.value && endDateInput.value) {
        const start = new Date(startDateInput.value);
        const end = new Date(endDateInput.value);

        if (end >= start) {
            const days = Math.ceil((end - start) / (1000 * 60 * 60 * 24)) + 1;
            numberOfDays.value = days;
            validateLeaveBalance(days);
        }
    }
}

function handleLeaveTypeChange() {
    const leaveType = document.getElementById('leaveType').value;
    const startDateInput = document.getElementById('startDate');
    const endDateInput = document.getElementById('endDate');
    const endDateContainer = document.getElementById('endDateContainer');
    const hintElement = document.getElementById('leaveTypeHint');

    // Reset date inputs
    startDateInput.value = '';
    endDateInput.value = '';
    document.getElementById('numberOfDays').value = '';

    // Get today's date in YYYY-MM-DD format
    const today = new Date().toISOString().split('T')[0];

    // Remove previous constraints
    startDateInput.removeAttribute('min');
    startDateInput.removeAttribute('max');
    endDateInput.removeAttribute('min');
    endDateInput.removeAttribute('max');

    switch (leaveType) {
        case 'SingleDay':
            // Single day: future dates, end date = start date
            startDateInput.setAttribute('min', today);
            endDateContainer.style.display = 'none';
            endDateInput.required = false;
            hintElement.textContent = 'Select a single day for leave (future dates only)';
            hintElement.className = 'text-muted';
            break;

        case 'MultipleDay':
            // Multiple day: future dates, end date > start date
            startDateInput.setAttribute('min', today);
            endDateContainer.style.display = 'block';
            endDateInput.required = true;
            endDateInput.setAttribute('min', today);
            hintElement.textContent = 'Select start and end dates for multiple days leave';
            hintElement.className = 'text-muted';
            break;

        case 'PastDay':
            // Past day: only past dates
            startDateInput.setAttribute('max', today);
            endDateContainer.style.display = 'block';
            endDateInput.required = true;
            endDateInput.setAttribute('max', today);
            hintElement.textContent = 'Select past dates only for missed attendance';
            hintElement.className = 'text-warning';
            break;

        case 'Emergency':
            // Emergency: any date
            endDateContainer.style.display = 'block';
            endDateInput.required = true;
            hintElement.textContent = 'Emergency leave - any date allowed';
            hintElement.className = 'text-danger';
            break;

        default:
            endDateContainer.style.display = 'block';
            endDateInput.required = true;
            hintElement.textContent = '';
            break;
    }
}

function validateLeaveBalance(days) {
    const leaveType = document.getElementById('leaveType').value;

    // Mock leave balance - replace with actual data from server
    const leaveBalance = {
        'SingleDay': 12,
        'MultipleDay': 10,
        'PastDay': 5,
        'Emergency': 3
    };

    const available = leaveBalance[leaveType] || 0;

    if (days > available) {
        showToast(`Only ${available} days available for ${leaveType} leave`, 'warning');
    }
}

function resetForm() {
    const endDateContainer = document.getElementById('endDateContainer');
    const endDate = document.getElementById('endDate');
    const hint = document.getElementById('leaveTypeHint');

    if (endDateContainer) endDateContainer.style.display = 'block';
    if (endDate) endDate.required = true;
    if (hint) hint.textContent = '';
}

function filterLeaves(status) {
    const cards = document.querySelectorAll('.leave-card');
    let visibleCount = 0;

    cards.forEach(card => {
        const cardStatus = card.querySelector('.leave-status .badge');

        if (status === 'all' || cardStatus.textContent.includes(status)) {
            card.parentElement.style.display = 'block';
            visibleCount++;
        } else {
            card.parentElement.style.display = 'none';
        }
    });

    // Update active button
    document.querySelectorAll('.btn-group .btn').forEach(btn => {
        btn.classList.remove('active');
    });
    event.target.classList.add('active');

    if (visibleCount === 0) {
        showToast('No leaves found with this status', 'info');
    }
}

function cancelLeave(leaveId) {
    if (confirm('Are you sure you want to cancel this leave request?')) {
        // TODO: Send cancel request to server
        showToast('Leave request cancelled successfully!', 'success');

        // Remove the card from UI
        const card = document.querySelector(`[data-leave-id="${leaveId}"]`);
        if (card) {
            card.style.opacity = '0';
            setTimeout(() => card.remove(), 300);
        }
    }
}

// ================================================
// 5. Form Validation Functions
// ================================================

function validateLeaveForm() {
    const leaveType = document.getElementById('leaveType').value;
    const startDate = document.getElementById('startDate').value;
    // End date might be hidden/not required for SingleDay, handled in calculateLeaveDays but need to get value for validation
    let endDate = document.getElementById('endDate').value;
    const reason = document.getElementById('reason').value;
    const numberOfDays = parseInt(document.getElementById('numberOfDays').value) || 0;

    if (leaveType === 'SingleDay') {
        endDate = startDate; // For single day, dates are same
    }

    if (!leaveType) {
        showToast('Please select a leave type', 'warning');
        return false;
    }

    if (!startDate) {
        showToast('Please select start date', 'warning');
        return false;
    }

    if (leaveType !== 'SingleDay' && !endDate) {
        showToast('Please select end date', 'warning');
        return false;
    }

    if (!reason || reason.trim().length < 10) {
        showToast('Please provide a detailed reason (minimum 10 characters)', 'warning');
        return false;
    }

    const start = new Date(startDate);
    const end = new Date(endDate);
    const today = new Date();
    today.setHours(0, 0, 0, 0);

    if (start > end) {
        showToast('End date must be after start date', 'warning');
        return false;
    }

    // Specific Leave Type Validations
    if (leaveType === 'SingleDay' && numberOfDays !== 1) {
        showToast('Single Day Leave must be exactly 1 day', 'warning');
        return false;
    }

    if (leaveType === 'MultipleDay' && numberOfDays < 2) {
        showToast('Multiple Day Leave must be at least 2 days', 'warning');
        return false;
    }

    if (leaveType === 'PastDay') {
        if (start >= today || end >= today) {
            showToast('Past Day Leave requires past dates only', 'warning');
            return false;
        }
    } else if (leaveType !== 'Emergency') {
        // For non-PastDay and non-Emergency (Normal leaves), shouldn't be in past
        if (start < today) {
            showToast('Cannot apply leave for past dates (use Past Day Leave)', 'warning');
            return false;
        }
    }

    return true;
}

// ================================================
// 6. Table Functions
// ================================================

function sortTable(columnIndex) {
    const table = document.querySelector('.table');
    const tbody = table.querySelector('tbody');
    const rows = Array.from(tbody.querySelectorAll('tr'));

    rows.sort((a, b) => {
        const aValue = a.cells[columnIndex].textContent.trim();
        const bValue = b.cells[columnIndex].textContent.trim();

        // Try to parse as number
        const aNum = parseFloat(aValue);
        const bNum = parseFloat(bValue);

        if (!isNaN(aNum) && !isNaN(bNum)) {
            return aNum - bNum;
        }

        return aValue.localeCompare(bValue);
    });

    rows.forEach(row => tbody.appendChild(row));
}

function searchTable(searchTerm) {
    const table = document.querySelector('.table');
    const rows = table.querySelectorAll('tbody tr');
    let visibleCount = 0;

    rows.forEach(row => {
        const text = row.textContent.toLowerCase();

        if (text.includes(searchTerm.toLowerCase())) {
            row.style.display = '';
            visibleCount++;
        } else {
            row.style.display = 'none';
        }
    });

    if (visibleCount === 0) {
        showToast('No results found', 'info');
    }
}

// ================================================
// 7. Navigation Functions
// ================================================

function logout() {
    if (confirm('Are you sure you want to logout?')) {
        window.location.href = '/Auth/Logout';
    }
}

function goBack() {
    window.history.back();
}

// ================================================
// 8. Event Listeners Setup
// ================================================

document.addEventListener('DOMContentLoaded', function () {
    // Setup leave form listeners
    const startDateInput = document.getElementById('startDate');
    const endDateInput = document.getElementById('endDate');
    const leaveTypeInput = document.getElementById('leaveType');
    const resetButton = document.querySelector('button[type="reset"]');

    if (startDateInput) {
        startDateInput.addEventListener('change', calculateLeaveDays);
    }

    if (endDateInput) {
        endDateInput.addEventListener('change', calculateLeaveDays);
    }

    if (leaveTypeInput) {
        leaveTypeInput.addEventListener('change', handleLeaveTypeChange);
    }

    if (resetButton) {
        resetButton.addEventListener('click', function () {
            // setTimeout to let the form reset happen first, then our custom reset logic
            setTimeout(resetForm, 0);
        });
    }

    // Setup form submission
    const leaveForm = document.querySelector('form[asp-action="ApplyLeaves"]');
    if (leaveForm) {
        leaveForm.addEventListener('submit', function (e) {
            if (!validateLeaveForm()) {
                e.preventDefault();
            }
        });
    }

    // Setup dashboard
    if (document.querySelector('[data-page="dashboard"]')) {
        initializeDashboard();
    }

    // Setup TimeSheet page
    const currentTimeElement = document.getElementById('currentTime');
    if (currentTimeElement) {
        // Update time immediately
        updateCurrentTime();

        // Update time every second
        setInterval(updateCurrentTime, 1000);

        // Load attendance data from database first, then fall back to localStorage
        loadTodayAttendance();

        // Save timesheet data to localStorage before page unload
        window.addEventListener('beforeunload', function () {
            saveToLocalStorage('timesheetData', timesheetData);
        });
    }
});

// ================================================
// 9. Date Formatting Functions
// ================================================

function formatDate(date) {
    const options = { year: 'numeric', month: 'long', day: 'numeric' };
    return new Date(date).toLocaleDateString('en-US', options);
}

function formatTime(time) {
    const options = { hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: true };
    return new Date(time).toLocaleTimeString('en-US', options);
}

// ================================================
// 10. API Functions
// ================================================

async function fetchData(url) {
    try {
        const response = await fetch(url);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        return await response.json();
    } catch (error) {
        console.error('Error fetching data:', error);
        showToast('Error loading data. Please try again.', 'error');
        return null;
    }
}

async function postData(url, data) {
    try {
        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(data)
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        return await response.json();
    } catch (error) {
        console.error('Error posting data:', error);
        showToast('Error submitting data. Please try again.', 'error');
        return null;
    }
}

// ================================================
// 11. Animation Functions
// ================================================

function animateElement(element, animationClass) {
    element.classList.add(animationClass);
    element.addEventListener('animationend', function () {
        element.classList.remove(animationClass);
    }, { once: true });
}

// ================================================
// 12. Storage Functions
// ================================================

function saveToLocalStorage(key, value) {
    try {
        localStorage.setItem(key, JSON.stringify(value));
    } catch (error) {
        console.error('Error saving to localStorage:', error);
    }
}

function getFromLocalStorage(key) {
    try {
        const item = localStorage.getItem(key);
        return item ? JSON.parse(item) : null;
    } catch (error) {
        console.error('Error reading from localStorage:', error);
        return null;
    }
}

function removeFromLocalStorage(key) {
    try {
        localStorage.removeItem(key);
    } catch (error) {
        console.error('Error removing from localStorage:', error);
    }
}
