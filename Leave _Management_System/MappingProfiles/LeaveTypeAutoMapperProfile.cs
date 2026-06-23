using AutoMapper;
using Leave__Management_System.Data;
using Leave__Management_System.Models.LeaveTypes;

namespace Leave__Management_System.MappingProfiles
{
    public class LeaveTypeAutoMapperProfile : Profile
    {
        public LeaveTypeAutoMapperProfile() 
        {
            CreateMap<LeaveType, LeaveTypeReadOnlyVM>()
                .ForMember(dest => dest.NumberOfDays, opt=> opt.MapFrom(src => src.NumberOfDays));

            CreateMap<LeaveTypeCreateVM, LeaveType>();

            CreateMap<LeaveTypeEditVM, LeaveType>();
            CreateMap<LeaveType, LeaveTypeEditVM>();
        }
    }
}
