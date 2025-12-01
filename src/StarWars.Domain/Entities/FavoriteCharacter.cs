namespace StarWars.Domain.Entities;

public class FavoriteCharacter
{
    public Guid Id { get; set; }
    // Guardamos el nombre y la URL para no depender siempre de SWAPI
    public string Name { get; set; } = string.Empty; 
    public string Url { get; set; } = string.Empty;  // Servirá como identificador único externo
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}