using System.Net.Http.Json;
using Spectre.Console;

namespace StarWars.ConsoleClient;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient()
    {
        _http = new HttpClient { BaseAddress = new Uri("http://localhost:8080/api/") };
    }

    public async Task<List<Character>> GetCharactersAsync(int page = 1)
    {
        try 
        {
            return await _http.GetFromJsonAsync<List<Character>>($"characters?page={page}") ?? new();
        }
        catch (Exception ex) { AnsiConsole.MarkupLine($"[red]Error connecting to API: {ex.Message}[/]"); return new(); }
    }

    public async Task<List<Character>> SearchCharactersAsync(string query)
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Character>>($"characters/search?name={query}") ?? new();
        }
        catch (Exception) { return new(); }
    }

    public async Task AddFavoriteAsync(Character character)
    {
        var response = await _http.PostAsJsonAsync("characters/favorites", new { Name = character.Name, Url = character.Url });
        
        if (response.IsSuccessStatusCode)
            AnsiConsole.MarkupLine($"[green]Successfully added {character.Name} to favorites![/]");
        else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            AnsiConsole.MarkupLine($"[yellow]{character.Name} is already in your favorites.[/]");
        else
            AnsiConsole.MarkupLine($"[red]Error adding favorite: {response.StatusCode}[/]");
    }

    public async Task<List<Favorite>> GetFavoritesAsync()
    {
        try
        {
            return await _http.GetFromJsonAsync<List<Favorite>>("characters/favorites") ?? new();
        }
        catch (Exception) { return new(); }
    }
}