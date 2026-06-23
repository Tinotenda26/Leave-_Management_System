using System;
using System.Threading.Tasks;
using Leave__Management_System.Data;
using Leave__Management_System.Services.LeaveRequests;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Leave__Management_System.Tests
{
    public class LeaveRequestsServiceTests
    {
        private ApplicationDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task ApproveAsync_ReducesAllocationRemainingDays()
        {
            var db = CreateInMemoryDb();

            var user = new ApplicationUser { Id = "u1", UserName = "u1", Email = "u1@example.com" };
            var leaveType = new LeaveType { Id = 1, Name = "Annual", NumberOfDays = 10 };
            var period = new Period { Id = 1, Name = "2026", StartDate = DateOnly.FromDateTime(new DateTime(2026,1,1)), EndDate = DateOnly.FromDateTime(new DateTime(2026,12,31)) };
            db.Users.Add(user);
            db.LeaveTypes.Add(leaveType);
            db.Periods.Add(period);
            db.LeaveAllocations.Add(new LeaveAllocation { Id = 1, EmployeeId = "u1", LeaveTypeId = 1, PeriodId = 1, NumberOfDays = 10, RemainingDays = 10 });
            db.LeaveRequests.Add(new LeaveRequest { Id = 1, EmployeeId = "u1", LeaveTypeId = 1, PeriodId = 1, NumberOfDays = 3, Status = LeaveRequestStatus.Pending, RequestDate = DateTime.UtcNow });
            await db.SaveChangesAsync();

            var service = new LeaveRequestsService(db);

            var approved = await service.ApproveAsync(1, "approver");

            Assert.True(approved);

            var alloc = await db.LeaveAllocations.FindAsync(1);
            Assert.Equal(7, alloc.RemainingDays);

            var req = await db.LeaveRequests.FindAsync(1);
            Assert.Equal(LeaveRequestStatus.Approved, req.Status);
            Assert.Equal("approver", req.ApprovedById);
            Assert.NotNull(req.DecisionDate);
        }

        [Fact]
        public async Task DenyAsync_SetsRequestDenied()
        {
            var db = CreateInMemoryDb();

            var user = new ApplicationUser { Id = "u2", UserName = "u2", Email = "u2@example.com" };
            var leaveType = new LeaveType { Id = 2, Name = "Sick", NumberOfDays = 10 };
            var period = new Period { Id = 2, Name = "2026", StartDate = DateOnly.FromDateTime(new DateTime(2026,1,1)), EndDate = DateOnly.FromDateTime(new DateTime(2026,12,31)) };
            db.Users.Add(user);
            db.LeaveTypes.Add(leaveType);
            db.Periods.Add(period);
            db.LeaveRequests.Add(new LeaveRequest { Id = 2, EmployeeId = "u2", LeaveTypeId = 2, PeriodId = 2, NumberOfDays = 2, Status = LeaveRequestStatus.Pending, RequestDate = DateTime.UtcNow });
            await db.SaveChangesAsync();

            var service = new LeaveRequestsService(db);

            var denied = await service.DenyAsync(2, "approver");

            Assert.True(denied);

            var req = await db.LeaveRequests.FindAsync(2);
            Assert.Equal(LeaveRequestStatus.Denied, req.Status);
            Assert.Equal("approver", req.ApprovedById);
            Assert.NotNull(req.DecisionDate);
        }
    }
}
