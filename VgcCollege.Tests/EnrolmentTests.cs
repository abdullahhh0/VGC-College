using Microsoft.EntityFrameworkCore;
using VgcCollege.Web.Data;
using VgcCollege.Web.Models;
using Xunit;

namespace VgcCollege.Tests;

public class EnrolmentTests
{
    private ApplicationDbContext GetDb()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CanAddStudent()
    {
        var db = GetDb();
        db.StudentProfiles.Add(new StudentProfile { Name = "Ali", Email = "ali@test.com", Phone = "123", Address = "Dublin", IdentityUserId = "u1" });
        await db.SaveChangesAsync();
        Assert.Equal(1, db.StudentProfiles.Count());
    }

    [Fact]
    public async Task CanEnrolStudent()
    {
        var db = GetDb();
        var student = new StudentProfile { Name = "Ali", Email = "ali@test.com", Phone = "123", Address = "Dublin", IdentityUserId = "u1" };
        var branch = new Branch { Name = "Dublin", Address = "Main St" };
        var course = new Course { Name = "IT", Branch = branch, StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1) };
        db.StudentProfiles.Add(student);
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        db.CourseEnrolments.Add(new CourseEnrolment { StudentProfileId = student.Id, CourseId = course.Id, EnrolDate = DateTime.Now, Status = "Active" });
        await db.SaveChangesAsync();
        Assert.Equal(1, db.CourseEnrolments.Count());
    }

    [Fact]
    public async Task StudentCannotSeeUnreleasedExamResults()
    {
        var db = GetDb();
        var student = new StudentProfile { Name = "Ali", Email = "ali@test.com", Phone = "123", Address = "Dublin", IdentityUserId = "u1" };
        var branch = new Branch { Name = "Dublin", Address = "Main St" };
        var course = new Course { Name = "IT", Branch = branch, StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1) };
        db.StudentProfiles.Add(student);
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var exam = new Exam { CourseId = course.Id, Title = "Midterm", Date = DateTime.Now, MaxScore = 100, ResultsReleased = false };
        db.Exams.Add(exam);
        await db.SaveChangesAsync();

        db.ExamResults.Add(new ExamResult { ExamId = exam.Id, StudentProfileId = student.Id, Score = 80, Grade = "A" });
        await db.SaveChangesAsync();

        var visible = db.ExamResults.Include(r => r.Exam)
            .Where(r => r.StudentProfileId == student.Id && r.Exam.ResultsReleased)
            .ToList();

        Assert.Empty(visible);
    }

    [Fact]
    public async Task StudentCanSeeReleasedExamResults()
    {
        var db = GetDb();
        var student = new StudentProfile { Name = "Ali", Email = "ali@test.com", Phone = "123", Address = "Dublin", IdentityUserId = "u1" };
        var branch = new Branch { Name = "Dublin", Address = "Main St" };
        var course = new Course { Name = "IT", Branch = branch, StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1) };
        db.StudentProfiles.Add(student);
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var exam = new Exam { CourseId = course.Id, Title = "Final", Date = DateTime.Now, MaxScore = 100, ResultsReleased = true };
        db.Exams.Add(exam);
        await db.SaveChangesAsync();

        db.ExamResults.Add(new ExamResult { ExamId = exam.Id, StudentProfileId = student.Id, Score = 75, Grade = "B" });
        await db.SaveChangesAsync();

        var visible = db.ExamResults.Include(r => r.Exam)
            .Where(r => r.StudentProfileId == student.Id && r.Exam.ResultsReleased)
            .ToList();

        Assert.Single(visible);
    }

    [Fact]
    public async Task CanAddAttendance()
    {
        var db = GetDb();
        var student = new StudentProfile { Name = "Ali", Email = "ali@test.com", Phone = "123", Address = "Dublin", IdentityUserId = "u1" };
        var branch = new Branch { Name = "Dublin", Address = "Main St" };
        var course = new Course { Name = "IT", Branch = branch, StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1) };
        db.StudentProfiles.Add(student);
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var enrolment = new CourseEnrolment { StudentProfileId = student.Id, CourseId = course.Id, EnrolDate = DateTime.Now, Status = "Active" };
        db.CourseEnrolments.Add(enrolment);
        await db.SaveChangesAsync();

        db.AttendanceRecords.Add(new AttendanceRecord { CourseEnrolmentId = enrolment.Id, WeekNumber = 1, Present = true });
        await db.SaveChangesAsync();

        Assert.Equal(1, db.AttendanceRecords.Count());
    }

    [Fact]
    public async Task CanAddAssignmentResult()
    {
        var db = GetDb();
        var student = new StudentProfile { Name = "Ali", Email = "ali@test.com", Phone = "123", Address = "Dublin", IdentityUserId = "u1" };
        var branch = new Branch { Name = "Dublin", Address = "Main St" };
        var course = new Course { Name = "IT", Branch = branch, StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1) };
        db.StudentProfiles.Add(student);
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        var assignment = new Assignment { CourseId = course.Id, Title = "Lab 1", MaxScore = 100, DueDate = DateTime.Now };
        db.Assignments.Add(assignment);
        await db.SaveChangesAsync();

        db.AssignmentResults.Add(new AssignmentResult { AssignmentId = assignment.Id, StudentProfileId = student.Id, Score = 90, Feedback = "Good" });
        await db.SaveChangesAsync();

        Assert.Equal(1, db.AssignmentResults.Count());
    }

    [Fact]
    public async Task EnrolmentStatusIsActive()
    {
        var db = GetDb();
        var student = new StudentProfile { Name = "Ali", Email = "ali@test.com", Phone = "123", Address = "Dublin", IdentityUserId = "u1" };
        var branch = new Branch { Name = "Dublin", Address = "Main St" };
        var course = new Course { Name = "IT", Branch = branch, StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1) };
        db.StudentProfiles.Add(student);
        db.Courses.Add(course);
        await db.SaveChangesAsync();

        db.CourseEnrolments.Add(new CourseEnrolment { StudentProfileId = student.Id, CourseId = course.Id, EnrolDate = DateTime.Now, Status = "Active" });
        await db.SaveChangesAsync();

        var enrolment = db.CourseEnrolments.First();
        Assert.Equal("Active", enrolment.Status);
    }

    [Fact]
    public async Task CanAddBranch()
    {
        var db = GetDb();
        db.Branches.Add(new Branch { Name = "Cork", Address = "High St" });
        await db.SaveChangesAsync();
        Assert.Equal(1, db.Branches.Count());
    }
}