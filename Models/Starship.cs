using System.ComponentModel.DataAnnotations;

// Using string for majority of data
// Some data has either numeric or N/A, string avoids the uncertainty here. 
// Also avoids any possible precision issues on backend by keeping as a string.
public class Starship
{
    public int StarshipId { get; set; } // Primary Key
    
    [Required(ErrorMessage = "The Name field is required.")]
    public string Name { get; set; }  = string.Empty;
    
    [Required(ErrorMessage = "The Model field is required.")]
    public string Model { get; set; }  = string.Empty;
    
    [Required(ErrorMessage = "The Manufacturer field is required.")]
    public string Manufacturer { get; set; }  = string.Empty;
    
    [Required(ErrorMessage = "The Cost (Credits) field is required.")]
    public string CostInCredits { get; set; }  = string.Empty;
    
    [Required(ErrorMessage = "The Length field is required.")]
    public string Length { get; set; }  = string.Empty;
    
    [Required(ErrorMessage = "The Max Atmosphering Speed field is required.")]
    public string MaxAtmospheringSpeed { get; set; }  = string.Empty;
    
    [Required(ErrorMessage = "The Crew field is required.")]
    public string Crew { get; set; }  = string.Empty;
    
    [Required(ErrorMessage = "The Passengers field is required.")]
    public string Passengers { get; set; }  = string.Empty;
    
    [Required(ErrorMessage = "The Cargo Capacity field is required.")]
    public string CargoCapacity { get; set; }  = string.Empty;
    
    [Required(ErrorMessage = "The Consumables field is required.")]
    public string Consumables { get; set; }  = string.Empty;
    
    [Required(ErrorMessage = "The Hypedrive Rating field is required.")]
    public string HyperdriveRating { get; set; }  = string.Empty;
    
    [Required(ErrorMessage = "The MGLT field is required.")]
    public string MGLT { get; set; }  = string.Empty;
    
    [Required(ErrorMessage = "The Starship Class field is required.")]
    public string StarshipClass { get; set; }  = string.Empty;
    
    public string[] Pilots { get; set; } = [];
    
    public string[] Films { get; set; } = [];
    
    public DateTime Created { get; set; }
    
    public DateTime Edited { get; set; }
    
    public string URL { get; set; } = string.Empty;
    
    public string ImageURL { get; set; } = "https://via.placeholder.com/";
}
