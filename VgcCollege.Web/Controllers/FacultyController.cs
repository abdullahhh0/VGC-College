using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;

namespace VgcCollege.Web.Controllers;

[Authorize(Roles = "Faculty")]
public class FacultyController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public FacultyController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> MyStudents()
    {
        var userId = _userManager.GetUserId(User);
        var faculty = await _db.FacultyProfiles.FirstOrDefaultAsync(f => f.IdentityUserId == userId);
        if (faculty == null) return NotFound();

        var enrolments = await _db.CourseEnrolments
            .Include(e => e.StudentProfile)
            .Include(e => e.Course)
            .ToListAsync();

        return View(enrolments);
    }

    public async Task<IActionResult> Gradebook()
    {
        var results = await _db.AssignmentResults
            .Include(r => r.Assignment)
            .Include(r => r.StudentProfile)
            .ToListAsync();
        return View(results);
    }
}