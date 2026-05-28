namespace VgcCollege.Web.Models;

public class StudentProfile
{
    public int Id { get; set; }
    public string IdentityUserId { get; set; }

    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Address { get; set; }
}