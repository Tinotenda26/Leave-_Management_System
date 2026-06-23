using Leave__Management_System.Data;
using Leave__Management_System.Services;
using Leave__Management_System.Services.Email;
using Leave__Management_System.Services.LeaveTypes;
using Leave__Management_System.Services.LeaveAllocation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Leave__Management_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddScoped<ILeaveTypesService, LeaveTypesService>();
            builder.Services.AddScoped<IUserDeletionService, UserDeletionService>();
            builder.Services.AddTransient<IEmailSender, EmailSender>();
            builder.Services.AddScoped<ILeaveAllocationsService, LeaveAllocationsService>();
                        builder.Services.AddScoped<Leave__Management_System.Services.LeaveRequests.ILeaveRequestsService, Leave__Management_System.Services.LeaveRequests.LeaveRequestsService>();

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>() 
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseMigrationsEndPoint();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            // Enable authentication middleware so Identity can sign in users
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.MapRazorPages()
               .WithStaticAssets();

            // Seed default data (runtime seeding)
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    Leave__Management_System.Data.Seed.DatabaseSeeder.InitializeAsync(services).GetAwaiter().GetResult();
                    logger.LogInformation("Database seeding completed.");
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            app.Run();
        }
    }
}
