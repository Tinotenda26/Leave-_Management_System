using AutoMapper;
using Leave__Management_System.Models.LeaveAllocations;
using Leave__Management_System.Models.LeaveTypes;
using Leave__Management_System.Models.Periods;
using Leave__Management_System.Data;

namespace Leave__Management_System.MappingProfiles
{
    public class LeaveAllocationAutoMapperProfile : Profile
    {
        public LeaveAllocationAutoMapperProfile()
        {
            CreateMap<Leave__Management_System.Data.LeaveAllocation, LeaveAllocationViewModel>();
            CreateMap<Leave__Management_System.Data.Period, PeriodViewModel>();
            CreateMap<Leave__Management_System.Data.ApplicationUser, EmployeeListViewModel>()
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"));
        }
    }
}
