using Spectre.Console;
using StarWars.ConsoleClient;

var client = new ApiClient();

// Título Estilizado
AnsiConsole.Write(
    new FigletText("Star Wars API")
        .Color(Color.Yellow));

while (true)
{
    AnsiConsole.Write(new Rule("[yellow]Main Menu[/]"));

    // Menú interactivo
    var choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Select an option:")
            .PageSize(10)
            .AddChoices(new[] {
                "1. 📜 List Characters",
                "2. 🔍 Search Character",
                "3. ⭐ View Favorites",
                "4. ❌ Exit"
            }));

    switch (choice)
    {
        case "1. 📜 List Characters":
            await HandleListCharacters();
            break;
        case "2. 🔍 Search Character":
            await HandleSearch();
            break;
        case "3. ⭐ View Favorites":
            await HandleFavorites();
            break;
        case "4. ❌ Exit":
            return;
    }
    
    // Pausa antes de limpiar
    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[grey]Press any key to return to menu...[/]");
    Console.ReadKey(true);
    AnsiConsole.Clear();
}

// --- Métodos Manejadores ---

async Task HandleListCharacters()
{
    // Spinner de carga
    await AnsiConsole.Status()
        .StartAsync("Fetching characters from Tatooine...", async ctx =>
        {
            var characters = await client.GetCharactersAsync();
            RenderTable(characters);
        });
}

async Task HandleSearch()
{
    var name = AnsiConsole.Ask<string>("Enter character name to search:");
    
    List<Character> results = new();
    
    await AnsiConsole.Status()
        .StartAsync($"Searching for '{name}'...", async ctx =>
        {
            results = await client.SearchCharactersAsync(name);
        });

    if (results.Count == 0)
    {
        AnsiConsole.MarkupLine("[red]No characters found.[/]");
        return;
    }

    RenderTable(results);

    // Sub-menú para agregar a favoritos directamente desde la búsqueda
    if (AnsiConsole.Confirm("Do you want to add one of these to favorites?"))
    {
        var charChoice = AnsiConsole.Prompt(
            new SelectionPrompt<Character>()
                .Title("Select character:")
                .UseConverter(c => c.Name) // Muestra solo el nombre en la lista
                .AddChoices(results));

        await client.AddFavoriteAsync(charChoice);
    }
}

async Task HandleFavorites()
{
    await AnsiConsole.Status()
        .StartAsync("Retrieving your favorite characters...", async ctx =>
        {
            var favs = await client.GetFavoritesAsync();
            
            var table = new Table();
            table.Border(TableBorder.Rounded);
            table.AddColumn("[yellow]Name[/]");
            table.AddColumn("Date Added");

            foreach (var f in favs)
            {
                table.AddRow(f.Name, f.AddedAt.ToLocalTime().ToString("g"));
            }

            AnsiConsole.Write(table);
        });
}

void RenderTable(List<Character> characters)
{
    var table = new Table();
    table.Border(TableBorder.Rounded);
    table.AddColumn("[yellow]Name[/]");
    table.AddColumn("Gender");
    table.AddColumn("Birth Year");

    foreach (var c in characters)
    {
        table.AddRow(
            c.Name ?? "[red]Unknown[/]", 
            c.Gender ?? "-", 
            c.BirthYear ?? "-"
        );
    }

    AnsiConsole.Write(table);
}