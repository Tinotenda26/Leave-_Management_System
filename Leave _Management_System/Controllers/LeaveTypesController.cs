using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Leave__Management_System.Data;
using Leave__Management_System.Models.LeaveTypes;
using AutoMapper;
using Leave__Management_System.Common;
using Leave__Management_System.Services.LeaveAllocation;

[Authorize(Roles = Roles.Supervisor + "," + Roles.Employee)]
public class LeaveTypesController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILeaveAllocationsService _leaveAllocationsService;
    private const string NameExistsValidationMessage = "A leave type with this name already exists in the database.";

    public LeaveTypesController(ApplicationDbContext context, IMapper mapper, ILeaveAllocationsService leaveAllocationsService)
    {
        _context = context;
        this._mapper = mapper;
        _leaveAllocationsService = leaveAllocationsService;
    }

    // GET: LEAVETYPES
    [Authorize(Roles = Roles.Supervisor + "," + Roles.Employee)]
    public async Task<IActionResult> Index()    
    {
        var data = await _context.LeaveTypes.ToListAsync();

        var viewData= _mapper.Map<List<LeaveTypeReadOnlyVM>>(data);
        return View(viewData);
    }

    // GET: LEAVETYPES/Details/5
    [Authorize(Roles = Roles.Supervisor + "," + Roles.Employee)]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var leavetype = await _context.LeaveTypes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (leavetype == null)
        {
            return NotFound();
        }

        var viewData = _mapper.Map<LeaveTypeReadOnlyVM>(leavetype);



        return View(viewData);
    }

    // GET: LEAVETYPES/Create
    [Authorize(Roles = Roles.Supervisor + "," + Roles.Employee)]
    public IActionResult Create()
    {
        return View();
    }

    // POST: LEAVETYPES/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.Supervisor + "," + Roles.Employee)]
    public async Task<IActionResult> Create([Bind("Id,Name,NumberOfDays")] LeaveTypeCreateVM leavetypeCreate)
    {
        if (await CheckIfLeaveTypeNameExists(leavetypeCreate.Name))
        {
            ModelState.AddModelError(nameof(leavetypeCreate.Name), NameExistsValidationMessage);
        }

        if (ModelState.IsValid)
        {
            var leaveType = _mapper.Map<LeaveType>(leavetypeCreate);
            _context.Add(leaveType);
            await _context.SaveChangesAsync();

            // Auto-allocate the new leave type to all employees for the current period
            string allocationMessage = "";
            try
            {
                await _leaveAllocationsService.AllocateLeaveTypeToAllEmployees(leaveType.Id);
                allocationMessage = $"Leave type '{leavetypeCreate.Name}' has been created and allocated to all employees.";
            }
            catch (InvalidOperationException ex)
            {
                allocationMessage = $"Leave type '{leavetypeCreate.Name}' created, but could not allocate to employees: {ex.Message}";
            }
            catch (Exception ex)
            {
                allocationMessage = $"Leave type '{leavetypeCreate.Name}' created successfully.";
            }

            TempData["Success"] = allocationMessage;
            return RedirectToAction(nameof(Index));
        }
        return View(leavetypeCreate);
    }

   

    // GET: LEAVETYPES/Edit/5
    [Authorize(Roles = Roles.Supervisor + "," + Roles.Employee)]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var leavetype = await _context.LeaveTypes.FindAsync(id);
        if (leavetype == null)
        {
            return NotFound();
        }
        var viewData = _mapper.Map<LeaveTypeEditVM>(leavetype);

        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            return PartialView("_EditPartial", viewData);
        }

        return View(viewData);
    }

    // POST: LEAVETYPES/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.Supervisor + "," + Roles.Employee)]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,NumberOfDays")] LeaveTypeEditVM leavetypeEditVM)
    {
        if (id == null)
        {
            return NotFound();
        }
        if (await CheckIfLeaveTypeNameExistsForEdit(leavetypeEditVM))
        {
            ModelState.AddModelError(nameof(leavetypeEditVM.Name), NameExistsValidationMessage);
        }


        if (ModelState.IsValid)
        {
            var leavetype = await _context.LeaveTypes.FindAsync(id);
            if (leavetype == null)
            {
                return NotFound();
            }

            leavetype.Name = leavetypeEditVM.Name;
            leavetype.NumberOfDays = leavetypeEditVM.NumberOfDays;

            try
            {
                var LeaveType= _mapper.Map<LeaveType>(leavetypeEditVM);
                _context.Update(leavetype);
                await _context.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaveTypeExists(leavetype.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }
        return View(leavetypeEditVM);
    }

    // GET: LEAVETYPES/Delete/5
    [Authorize(Roles = Roles.Supervisor + "," + Roles.Employee)]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var leavetype = await _context.LeaveTypes
            .FirstOrDefaultAsync(m => m.Id == id);
        if (leavetype == null)
        {
            return NotFound();
        }

        var viewData = _mapper.Map<LeaveTypeReadOnlyVM>(leavetype);
        return View(viewData);
    }

    // POST: LEAVETYPES/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = Roles.Supervisor + "," + Roles.Employee)]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var leavetype = await _context.LeaveTypes.FindAsync(id);
        if (leavetype != null)
        {
            _context.LeaveTypes.Remove(leavetype);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool LeaveTypeExists(int? id)
    {
        return _context.LeaveTypes.Any(e => e.Id == id);
    }

    private async Task<bool> CheckIfLeaveTypeNameExists(string name)
    {
        var lowerCaseName = name.ToLower();
        return _context.LeaveTypes.Any(e => e.Name.ToLower().Equals(lowerCaseName));
    }

    private async Task<bool> CheckIfLeaveTypeNameExistsForEdit(LeaveTypeEditVM leavetypeEditVM)
    {
        var lowerCaseName = leavetypeEditVM.Name.ToLower();
        return await _context.LeaveTypes.AnyAsync(e => e.Name.ToLower().Equals(lowerCaseName) && e.Id != leavetypeEditVM.Id);
    }
}
