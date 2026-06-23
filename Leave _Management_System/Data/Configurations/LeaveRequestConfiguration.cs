using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Leave__Management_System.Data.Configurations
{
    public class LeaveRequestConfiguration : IEntityTypeConfiguration<LeaveRequest>
    {
        public void Configure(EntityTypeBuilder<LeaveRequest> builder)
        {
            builder.HasKey(lr => lr.Id);

            builder.Property(lr => lr.StartDate)
                .IsRequired();

            builder.Property(lr => lr.EndDate)
                .IsRequired();

            builder.Property(lr => lr.NumberOfDays)
                .IsRequired();

            builder.Property(lr => lr.RequestDate)
                .IsRequired();

            builder.Property(lr => lr.Status)
                .HasConversion<int>()
                .HasDefaultValue(LeaveRequestStatus.Pending);

            // Relationships
            builder.HasOne(lr => lr.Employee)
                .WithMany()
                .HasForeignKey(lr => lr.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(lr => lr.ApprovedBy)
                .WithMany()
                .HasForeignKey(lr => lr.ApprovedById)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(lr => lr.LeaveType)
                .WithMany()
                .HasForeignKey(lr => lr.LeaveTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(lr => lr.Period)
                .WithMany()
                .HasForeignKey(lr => lr.PeriodId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}