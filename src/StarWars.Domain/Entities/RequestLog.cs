namespace StarWars.Domain.Entities;

public class RequestLog
{
    public Guid Id { get; set; }
    public string HttpMethod { get; set; } = string.Empty; // GET, POST, etc.
    public string Endpoint { get; set; } = string.Empty;   // /api/characters/search
    public string? QueryString { get; set; }               // ?name=luke
    public int StatusCode { get; set; }                    // 200, 404, 500
    public DateTime Timestamp { get; set; }                // Cuándo ocurrió (UTC)
    public long DurationMs { get; set; }                   // Cuánto tardó (milisegundos)
    public string? IpAddress { get; set; }
}