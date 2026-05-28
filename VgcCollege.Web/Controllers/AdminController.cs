using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;

    public AdminController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Branches()
    {
        return View(await _db.Branches.ToListAsync());
    }

    public IActionResult CreateBranch() => View();

    [HttpPost]
    public async Task<IActionResult> CreateBranch(Branch branch)
    {
        if (!ModelState.IsValid) return View(branch);
        _db.Branches.Add(branch);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Branches));
    }

    public async Task<IActionResult> EditBranch(int id)
    {
        var branch = await _db.Branches.FindAsync(id);
        if (branch == null) return NotFound();
        return View(branch);
    }

    [HttpPost]
    public async Task<IActionResult> EditBranch(Branch branch)
    {
        _db.Branches.Update(branch);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Branches));
    }

    public async Task<IActionResult> DeleteBranch(int id)
    {
        var branch = await _db.Branches.FindAsync(id);
        if (branch != null) _db.Branches.Remove(branch);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Branches));
    }

    public async Task<IActionResult> Courses()
    {
        return View(await _db.Courses.Include(c => c.Branch).ToListAsync());
    }

    public async Task<IActionResult> CreateCourse()
    {
        ViewBag.Branches = await _db.Branches.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse(Course course)
    {
        ModelState.Remove("Branch");
        if (!ModelState.IsValid)
        {
            ViewBag.Branches = await _db.Branches.ToListAsync();
            return View(course);
        }
        _db.Courses.Add(course);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Courses));
    }

    public async Task<IActionResult> EditCourse(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course == null) return NotFound();
        ViewBag.Branches = await _db.Branches.ToListAsync();
        return View(course);
    }

    [HttpPost]
    public async Task<IActionResult> EditCourse(Course course)
    {
        ModelState.Remove("Branch");
        _db.Courses.Update(course);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Courses));
    }

    public async Task<IActionResult> DeleteCourse(int id)
    {
        var course = await _db.Courses.FindAsync(id);
        if (course != null) _db.Courses.Remove(course);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Courses));
    }

    public async Task<IActionResult> Students()
    {
        return View(await _db.StudentProfiles.ToListAsync());
    }

    public async Task<IActionResult> Enrolments()
    {
        return View(await _db.CourseEnrolments
            .Include(e => e.StudentProfile)
            .Include(e => e.Course)
            .ToListAsync());
    }

    public async Task<IActionResult> CreateEnrolment()
    {
        ViewBag.Students = await _db.StudentProfiles.ToListAsync();
        ViewBag.Courses = await _db.Courses.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateEnrolment(CourseEnrolment enrolment)
    {
        ModelState.Remove("StudentProfile");
        ModelState.Remove("Course");
        if (!ModelState.IsValid)
        {
            ViewBag.Students = await _db.StudentProfiles.ToListAsync();
            ViewBag.Courses = await _db.Courses.ToListAsync();
            return View(enrolment);
        }
        enrolment.EnrolDate = DateTime.Now;
        _db.CourseEnrolments.Add(enrolment);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Enrolments));
    }

    public async Task<IActionResult> EditEnrolment(int id)
    {
        var enrolment = await _db.CourseEnrolments
            .Include(e => e.StudentProfile)
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (enrolment == null) return NotFound();
        return View(enrolment);
    }

    [HttpPost]
    public async Task<IActionResult> EditEnrolment(int id, string Status)
    {
        var enrolment = await _db.CourseEnrolments.FindAsync(id);
        if (enrolment == null) return NotFound();
        enrolment.Status = Status;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Enrolments));
    }

    public async Task<IActionResult> DeleteEnrolment(int id)
    {
        var enrolment = await _db.CourseEnrolments.FindAsync(id);
        if (enrolment != null) _db.CourseEnrolments.Remove(enrolment);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Enrolments));
    }

    public async Task<IActionResult> Attendance()
    {
        return View(await _db.AttendanceRecords
            .Include(a => a.CourseEnrolment)
            .ThenInclude(e => e.StudentProfile)
            .Include(a => a.CourseEnrolment)
            .ThenInclude(e => e.Course)
            .ToListAsync());
    }

    public async Task<IActionResult> CreateAttendance()
    {
        ViewBag.Enrolments = await _db.CourseEnrolments
            .Include(e => e.StudentProfile)
            .Include(e => e.Course)
            .ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAttendance(int CourseEnrolmentId, int WeekNumber, bool Present)
    {
        var record = new AttendanceRecord
        {
            CourseEnrolmentId = CourseEnrolmentId,
            WeekNumber = WeekNumber,
            Present = Present
        };
        _db.AttendanceRecords.Add(record);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Attendance));
    }

    public async Task<IActionResult> DeleteAttendance(int id)
    {
        var record = await _db.AttendanceRecords.FindAsync(id);
        if (record != null) _db.AttendanceRecords.Remove(record);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Attendance));
    }

    public async Task<IActionResult> Exams()
    {
        return View(await _db.Exams.Include(e => e.Course).ToListAsync());
    }

    public async Task<IActionResult> CreateExam()
    {
        ViewBag.Courses = await _db.Courses.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateExam(Exam exam)
    {
        ModelState.Remove("Course");
        if (!ModelState.IsValid)
        {
            ViewBag.Courses = await _db.Courses.ToListAsync();
            return View(exam);
        }
        _db.Exams.Add(exam);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Exams));
    }

    public async Task<IActionResult> DeleteExam(int id)
    {
        var exam = await _db.Exams.FindAsync(id);
        if (exam != null) _db.Exams.Remove(exam);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Exams));
    }

    public async Task<IActionResult> ReleaseResults(int id)
    {
        var exam = await _db.Exams.FindAsync(id);
        if (exam == null) return NotFound();
        exam.ResultsReleased = true;
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Exams));
    }

    public async Task<IActionResult> ExamResults()
    {
        return View(await _db.ExamResults
            .Include(r => r.Exam)
            .Include(r => r.StudentProfile)
            .ToListAsync());
    }

    public async Task<IActionResult> CreateExamResult()
    {
        ViewBag.Exams = await _db.Exams.ToListAsync();
        ViewBag.Students = await _db.StudentProfiles.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateExamResult(ExamResult result)
    {
        ModelState.Remove("Exam");
        ModelState.Remove("StudentProfile");
        _db.ExamResults.Add(result);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(ExamResults));
    }

    public async Task<IActionResult> DeleteExamResult(int id)
    {
        var result = await _db.ExamResults.FindAsync(id);
        if (result != null) _db.ExamResults.Remove(result);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(ExamResults));
    }

    public async Task<IActionResult> Assignments()
    {
        return View(await _db.Assignments.Include(a => a.Course).ToListAsync());
    }

    public async Task<IActionResult> CreateAssignment()
    {
        ViewBag.Courses = await _db.Courses.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssignment(Assignment assignment)
    {
        ModelState.Remove("Course");
        if (!ModelState.IsValid)
        {
            ViewBag.Courses = await _db.Courses.ToListAsync();
            return View(assignment);
        }
        _db.Assignments.Add(assignment);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Assignments));
    }

    public async Task<IActionResult> AssignmentResults()
    {
        return View(await _db.AssignmentResults
            .Include(r => r.Assignment)
            .Include(r => r.StudentProfile)
            .ToListAsync());
    }

    public async Task<IActionResult> CreateAssignmentResult()
    {
        ViewBag.Assignments = await _db.Assignments.ToListAsync();
        ViewBag.Students = await _db.StudentProfiles.ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAssignmentResult(AssignmentResult result)
    {
        ModelState.Remove("Assignment");
        ModelState.Remove("StudentProfile");
        _db.AssignmentResults.Add(result);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(AssignmentResults));
    }

    public async Task<IActionResult> DeleteAssignmentResult(int id)
    {
        var result = await _db.AssignmentResults.FindAsync(id);
        if (result != null) _db.AssignmentResults.Remove(result);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(AssignmentResults));
    }
}