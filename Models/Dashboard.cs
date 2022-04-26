namespace Healthometer_API.Models;

public class Dashboard
{
    public List<MedicalVisit>? RegularVisits { set; get; }
    public float? TakenSpace { set; get; }
    public List<Document> Documents { set; get; }
    public List<FamilyMember> Family { get; set; }
}