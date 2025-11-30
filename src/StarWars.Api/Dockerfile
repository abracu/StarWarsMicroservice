# Etapa 1: Construcción
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar solución y proyectos para restaurar dependencias
# Nota: Copiamos los .csproj primero para aprovechar el caché de capas de Docker
COPY *.sln .
COPY src/StarWars.Api/*.csproj ./src/StarWars.Api/
COPY src/StarWars.Application/*.csproj ./src/StarWars.Application/
COPY src/StarWars.Domain/*.csproj ./src/StarWars.Domain/
COPY src/StarWars.Infrastructure/*.csproj ./src/StarWars.Infrastructure/
COPY src/StarWars.ConsoleClient/*.csproj ./src/StarWars.ConsoleClient/
COPY tests/StarWars.UnitTests/*.csproj ./tests/StarWars.UnitTests/

RUN dotnet restore

# Copiar el resto del código
COPY . .

# Publicar la API
WORKDIR /app/src/StarWars.Api
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: Runtime (Imagen ligera para producción)
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "StarWars.Api.dll"]