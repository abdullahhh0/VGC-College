namespace VgcCollege.Web.Models;
public class Course
{
    public int Id { get; set; }
    public string Name { get; set; }

    public int BranchId { get; set; }
    public Branch Branch { get; set; }

    public DateTime StartDate { get; set; } = DateTime.Now;
    public DateTime EndDate { get; set; } = DateTime.Now.AddYears(1);
}