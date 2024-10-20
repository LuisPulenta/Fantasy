using System.ComponentModel.DataAnnotations;
using Fantasy.Shared.Resources;

namespace Fantasy.Shared.Entities;

public class Tournament
{
    public int Id { get; set; }

    [Display(Name = "Tournament", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    public string? Image { get; set; }

    [Display(Name = "IsActive", ResourceType = typeof(Literals))]
    public bool IsActive { get; set; }

    [Display(Name = "Remarks", ResourceType = typeof(Literals))]
    public string? Remarks { get; set; }

    [Display(Name = "Imagen")]
    public string ImageFull => string.IsNullOrEmpty(Image)
? $"https://localhost:7033/images/noimage.png"
: $"https://localhost:7033{Image[1..]}";

    public ICollection<TournamentTeam>? TournamentTeams { get; set; }

    public int TeamsCount => TournamentTeams == null ? 0 : TournamentTeams.Count;

    public ICollection<Match>? Matches { get; set; }

    public int MatchesCount => Matches == null ? 0 : Matches.Count;

    public ICollection<Group>? Groups { get; set; }

    public int GroupsCount => Groups == null ? 0 : Groups.Count;

    public ICollection<Prediction>? Predictions { get; set; }

    public int PredictionsCount => Predictions == null ? 0 : Predictions.Count;

}
