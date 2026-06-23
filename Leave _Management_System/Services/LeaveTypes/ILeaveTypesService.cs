using Leave__Management_System.Models.LeaveTypes;

namespace Leave__Management_System.Services.LeaveTypes;

public interface ILeaveTypesService
{
    Task Create(LeaveTypeCreateVM model);
    Task Edit(LeaveTypeEditVM model);
    Task<T?> Get<T>(int id) where T : class;
    Task<List<LeaveTypeReadOnlyVM>> GetAll();
    Task Remove(int id);
}