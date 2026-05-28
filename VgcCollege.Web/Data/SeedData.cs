using Microsoft.AspNetCore.Identity;
using VgcCollege.Web.Models;

namespace VgcCollege.Web.Data;

public static class SeedData
{
    public static async Task InitialiseAsync(IServiceProvider services)
    {
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var db = services.GetRequiredService<ApplicationDbContext>();D:
        

        string[] roles = { "Admin", "Faculty", "Student" };
        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));

        await CreateUser(userManager, "Abdullah@vgc.ie", "Abdullah@026", "Admin", db, isAdmin: true);
        var facultyUser = await CreateUser(userManager, "Hafiz@vgc.ie", "Hafiz@026", "Faculty", db);
        await CreateUser(userManager, "student1@vgc.ie", "Student@12345", "Student", db);
        await CreateUser(userManager, "student2@vgc.ie", "Student@12345", "Student", db);

        if (!db.Branches.Any())
        {
            db.Branches.AddRange(
                new Branch { Name = "Dublin", Address = "1 Main St, Dublin" },
                new Branch { Name = "Cork", Address = "2 High St, Cork" },
                new Branch { Name = "Galway", Address = "3 Shop St, Galway" }
            );
            await db.SaveChangesAsync();
        }

        if (!db.Courses.Any())
        {
            db.Courses.AddRange(
                new Course { Name = "Software Development", BranchId = 1, StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1) },
                new Course { Name = "Business Management", BranchId = 2, StartDate = DateTime.Now, EndDate = DateTime.Now.AddYears(1) }
            );
            await db.SaveChangesAsync();
        }

        
        if (!db.Assignments.Any())
        {
            db.Assignments.AddRange(
                new Assignment { CourseId = 1, Title = "Assignment 1", MaxScore = 100, DueDate = DateTime.Now.AddMonths(1) },
                new Assignment { CourseId = 1, Title = "Assignment 2", MaxScore = 100, DueDate = DateTime.Now.AddMonths(2) }
            );
            await db.SaveChangesAsync();
        }

        if (!db.AssignmentResults.Any())
        {
            var student1 = db.StudentProfiles.FirstOrDefault(s => s.Email == "student1@vgc.ie");
            var student2 = db.StudentProfiles.FirstOrDefault(s => s.Email == "student2@vgc.ie");

            if (student1 != null)
            {
                db.AssignmentResults.AddRange(
                    new AssignmentResult { AssignmentId = 1, StudentProfileId = student1.Id, Score = 85, Feedback = "Good work!" },
                    new AssignmentResult { AssignmentId = 2, StudentProfileId = student1.Id, Score = 90, Feedback = "Excellent!" }
                );
            }
            if (student2 != null)
            {
                db.AssignmentResults.Add(
                    new AssignmentResult { AssignmentId = 1, StudentProfileId = student2.Id, Score = 75, Feedback = "Well done!" }
                );
            }
            await db.SaveChangesAsync();
        }
    } 

    private static async Task<IdentityUser> CreateUser(UserManager<IdentityUser> userManager,
        string email, string password, string role,
        ApplicationDbContext db, bool isAdmin = false)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            await userManager.CreateAsync(user, password);
            await userManager.AddToRoleAsync(user, role);

            if (role == "Faculty")
                db.FacultyProfiles.Add(new FacultyProfile { IdentityUserId = user.Id, Name = "Faculty User", Email = email, Phone = "0871234567" });
            else if (role == "Student")
            {
                var count = db.StudentProfiles.Count() + 1;
                db.StudentProfiles.Add(new StudentProfile { IdentityUserId = user.Id, Name = $"Student {count}", Email = email, Phone = "0861234567", Address = "Dublin" });
            }
            await db.SaveChangesAsync();
        }
        return user;
    }
}