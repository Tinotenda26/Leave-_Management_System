using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Leave__Management_System.Data;
using Leave__Management_System.Models.LeaveTypes;
using Leave__Management_System.LeaveAllocation;

namespace Leave__Management_System.Services.LeaveTypes;

public class LeaveTypesService : ILeaveTypesService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public LeaveTypesService(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<LeaveTypeReadOnlyVM>> GetAll()
    {
        var data = await _context.LeaveTypes.ToListAsync();
        var viewData = _mapper.Map<List<LeaveTypeReadOnlyVM>>(data);
        return viewData;
    }

    public async Task<T?> Get<T>(int id) where T : class
    {
        var data = await _context.LeaveTypes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (data == null)
        {
            return null;
        }
        var viewData = _mapper.Map<T>(data);
        return viewData;
    }

    public async Task Remove(int id)
    {
        var data = await _context.LeaveTypes.FirstOrDefaultAsync(m => m.Id == id);
        if (data != null)
        {
            _context.LeaveTypes.Remove(data);
            await _context.SaveChangesAsync();
        }
    }

    public async Task Edit(LeaveTypeEditVM model)
    {
        var leaveType = _mapper.Map<LeaveType>(model);
        _context.Update(leaveType);
        await _context.SaveChangesAsync();
    }

    public async Task Create(LeaveTypeCreateVM model)
    {
        var leaveType = _mapper.Map<LeaveType>(model);
        _context.LeaveTypes.Add(leaveType);
        await _context.SaveChangesAsync();

        // After creating a new leave type, create allocations for existing users for current periods
        var periods = await _context.Periods.ToListAsync();
        var users = await _context.Users.ToListAsync();

        if (periods.Any() && users.Any())
        {
            foreach (var period in periods)
            {
                // calculate months remaining based on current date and period end
                var currentDate = DateTime.Now;
                var monthsRemaining = period.EndDate.Month - currentDate.Month;
                if (monthsRemaining <= 0) monthsRemaining = 12; // fallback to full allocation

                var accuralRate = decimal.Divide(leaveType.NumberOfDays, 12);
                var calculated = (int)Math.Ceiling(accuralRate * monthsRemaining);

                foreach (var user in users)
                {
                    var exists = await _context.LeaveAllocations.AnyAsync(a => a.EmployeeId == user.Id && a.LeaveTypeId == leaveType.Id && a.PeriodId == period.Id);
                    if (!exists)
                    {
                        var allocation = new Leave__Management_System.Data.LeaveAllocation
                        {
                            EmployeeId = user.Id,
                            LeaveTypeId = leaveType.Id,
                            PeriodId = period.Id,
                            NumberOfDays = calculated,
                            RemainingDays = calculated
                        };
                        _context.LeaveAllocations.Add(allocation);
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}




