using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;

namespace VgcCollege.Web.Controllers;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public StudentController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task<IActionResult> Dashboard()
    {
        var userId = _userManager.GetUserId(User);
        var student = await _db.StudentProfiles.FirstOrDefaultAsync(s => s.IdentityUserId == userId);
        if (student == null) return NotFound();

        var enrolments = await _db.CourseEnrolments
            .Include(e => e.Course)
            .Where(e => e.StudentProfileId == student.Id)
            .ToListAsync();

        return View(enrolments);
    }

    public async Task<IActionResult> MyResults()
    {
        var userId = _userManager.GetUserId(User);
        var student = await _db.StudentProfiles.FirstOrDefaultAsync(s => s.IdentityUserId == userId);
        if (student == null) return NotFound();

        var results = await _db.ExamResults
            .Include(r => r.Exam)
            .Where(r => r.StudentProfileId == student.Id && r.Exam.ResultsReleased)
            .ToListAsync();

        return View(results);
    }

    public async Task<IActionResult> MyAssignments()
    {
        var userId = _userManager.GetUserId(User);
        var student = await _db.StudentProfiles.FirstOrDefaultAsync(s => s.IdentityUserId == userId);
        if (student == null) return NotFound();

        var results = await _db.AssignmentResults
            .Include(r => r.Assignment)
            .Where(r => r.StudentProfileId == student.Id)
            .ToListAsync();

        return View(results);
    }
}