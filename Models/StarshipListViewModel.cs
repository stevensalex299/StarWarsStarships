public class StarshipListViewModel
{
    public required IEnumerable<Starship> Starships { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}