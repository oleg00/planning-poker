namespace PlanningPoker.Data;

public class PlanningUserPage
{
    public string Name { get; init; } = string.Empty;
    public double CardValue { get; set; }
    public bool HasVoted => CardValue != 0;
}