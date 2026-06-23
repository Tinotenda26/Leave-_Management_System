using System;
using Leave__Management_System.Data;
using Microsoft.AspNetCore.Identity;

namespace Leave__Management_System.Data.Seed
{
    public static class LeaveRequestSeed
    {
        public static void Seed(ModelBuilder builder)
        {
            // create a sample leave request for the seeded user
            builder.Entity<LeaveRequest>().HasData(new LeaveRequest
            {
                Id = 1,
                EmployeeId = "a1b2c3d4-5e6f-7g8h-9i0j-1k2l3m4n5o6p",
                LeaveTypeId = 1,
                PeriodId = 1,
                StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(7)),
                EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(10)),
                NumberOfDays = 3,
                RequestDate = DateTime.UtcNow,
                Status = LeaveRequestStatus.Pending
            });
        }
    }
}
