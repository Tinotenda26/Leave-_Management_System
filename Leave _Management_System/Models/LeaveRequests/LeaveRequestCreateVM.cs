using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Leave__Management_System.Models.LeaveRequests
{
    public class LeaveRequestCreateVM : IValidatableObject
    {
        public int LeaveAllocationId { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; } = string.Empty;
        public int PeriodId { get; set; }
        public DateOnly PeriodStart { get; set; }
        public DateOnly PeriodEnd { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateOnly StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateOnly EndDate { get; set; }

        [Required(ErrorMessage = "Number of days is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Number of days must be at least 1")]
        [Display(Name = "Number of Days")]
        public int NumberOfDays { get; set; }

        /// <summary>
        /// Maximum allowed days (current remaining)
        /// </summary>
        public int MaxDays { get; set; }

        /// <summary>
        /// Original entitlement for this allocation (does not change when requests are made)
        /// </summary>
        public int OriginalDays { get; set; }

        /// <summary>
        /// Select list for available leave types
        /// </summary>
        public List<SelectListItem> LeaveTypeOptions { get; set; } = new List<SelectListItem>();

        /// <summary>
        /// Additional information/remarks about the leave request
        /// </summary>
        [StringLength(500, ErrorMessage = "Additional information cannot exceed 500 characters")]
        [Display(Name = "Additional Information")]
        public string? AdditionalInformation { get; set; }

        /// <summary>
        /// Validate the leave request according to business rules
        /// </summary>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var errors = new List<ValidationResult>();

            // Determine effective period: if PeriodStart/End are uninitialized, default to today..2200-12-31
            var today = DateOnly.FromDateTime(DateTime.Today);
            var defaultPeriodStart = today;
            var defaultPeriodEnd = new DateOnly(2200, 12, 31);

            var effectivePeriodStart = PeriodStart == DateOnly.MinValue ? defaultPeriodStart : PeriodStart;
            var effectivePeriodEnd = PeriodEnd == DateOnly.MinValue ? defaultPeriodEnd : PeriodEnd;

            // Validate date range
            if (StartDate < effectivePeriodStart || StartDate > effectivePeriodEnd)
            {
                errors.Add(new ValidationResult(
                    $"Start date must be within the period ({effectivePeriodStart:yyyy-MM-dd} to {effectivePeriodEnd:yyyy-MM-dd})",
                    new[] { nameof(StartDate) }));
            }

            if (EndDate < effectivePeriodStart || EndDate > effectivePeriodEnd)
            {
                errors.Add(new ValidationResult(
                    $"End date must be within the period ({effectivePeriodStart:yyyy-MM-dd} to {effectivePeriodEnd:yyyy-MM-dd})",
                    new[] { nameof(EndDate) }));
            }

            // Validate start date is before or equal to end date
            if (StartDate > EndDate)
            {
                errors.Add(new ValidationResult(
                    "Start date must be before or equal to end date",
                    new[] { nameof(StartDate), nameof(EndDate) }));
            }

            // Validate number of days matches date range (accounting for inclusive range)
            int calculatedDays = (EndDate.DayNumber - StartDate.DayNumber) + 1;
            if (NumberOfDays != calculatedDays)
            {
                errors.Add(new ValidationResult(
                    $"Number of days ({NumberOfDays}) does not match the selected date range ({calculatedDays} days)",
                    new[] { nameof(NumberOfDays) }));
            }

            // Validate number of days does not exceed remaining days (only enforce when MaxDays > 0)
            if (MaxDays > 0 && NumberOfDays > MaxDays)
            {
                errors.Add(new ValidationResult(
                    $"You cannot request more than {MaxDays} days. You have requested {NumberOfDays} days.",
                    new[] { nameof(NumberOfDays) }));
            }

            // Validate that dates are not in the past (optional - adjust as needed)
            if (StartDate < today || EndDate < today)
            {
                errors.Add(new ValidationResult(
                    "Cannot request leave for dates in the past",
                    new[] { nameof(StartDate), nameof(EndDate) }));
            }

            return errors;
        }
    }
}
