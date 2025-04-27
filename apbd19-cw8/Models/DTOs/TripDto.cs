namespace apbd19_cw8.Models.DTOs;

public class TripDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public int MaxPeople { get; set; }
    public List<CountryDto> Countries { get; set; }
}

public class CountryDto
{
    public string Name { get; set; }
}