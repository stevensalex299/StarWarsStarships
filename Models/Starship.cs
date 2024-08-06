using System.Text.Json.Serialization;

// Using string for majority of data
// Some data has either numeric or N/A, string avoids the uncertainty here. 
// Also avoids any possible precision issues on backend by keeping as a string.
public class Starship
{
    public int StarshipId { get; set; } // Primary Key
    
    public required string Name { get; set; }
    
    public required string Model { get; set; }
    
    public required string Manufacturer { get; set; }
    
    public required string CostInCredits { get; set; }
    
    public required string Length { get; set; }
    
    public required string MaxAtmospheringSpeed { get; set; }
    
    public required string Crew { get; set; }
    
    public required string Passengers { get; set; }
    
    public required string CargoCapacity { get; set; }
    
    public required string Consumables { get; set; }
    
    public required string HyperdriveRating { get; set; }
    
    public required string MGLT { get; set; }
    
    public required string StarshipClass { get; set; }
    
    public required string[] Pilots { get; set; }
    
    public required string[] Films { get; set; }
    
    public DateTime Created { get; set; }
    
    public DateTime Edited { get; set; }
    
    public required string URL { get; set; }
}
