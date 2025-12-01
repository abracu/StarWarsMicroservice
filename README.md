

# Star Wars API Microservice 🌌

Una solución robusta y escalable construida sobre **.NET 8** siguiendo los principios de **Clean Architecture**. Este microservicio orquesta datos de la API pública de Star Wars (SWAPI), gestiona una persistencia local relacional y optimiza el rendimiento mediante caché distribuido.

## 🏗️ Arquitectura y Diseño

El proyecto está estructurado en capas concéntricas para garantizar la separación de responsabilidades y la testabilidad (Clean Architecture):

  - **Domain:** Entidades del núcleo (`FavoriteCharacter`, `RequestLog`) sin dependencias externas.
  - **Application:** Lógica de negocio, DTOs e Interfaces (`ISwapiService`).
  - **Infrastructure:** Implementación de persistencia (`EF Core`), Clientes HTTP, Caché Distribuido (`Redis`) y adaptadores externos.
  - **API:** Punto de entrada RESTful, configuración de contenedores y Middlewares.

## 🛠️ Stack Tecnológico
  - **Data** PostgreSQL 15
  - **Cache** Redis (Distributed Caching)
  - **ORM** Entity Framework Core (Code-First)
  - **Containerización** Docker & Docker Compose
  - **CLI Client** Spectre.Console
  - **Testing** xUnit

## 🚀 Quick Start

La solución es "Cloud-Native ready". No necesitas instalar el runtime de .NET ni bases de datos locales.

### Prerrequisitos

  * [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado.

### Pasos de Ejecución

1.  **Clonar el repositorio:**

    ```bash
    git clone <TU_URL_DEL_REPO>
    cd StarWarsMicroservice
    ```

2.  **Desplegar la infraestructura:**

    ```bash
    docker compose up --build
    ```

    > Esto levantará la API, la base de datos PostgreSQL y el servidor Redis.
    
3.  **Correr los Test**
    ```bash
    dotnet test
    ```
4.  **Acceder a la documentación:**
    Navega a: [http://localhost:8080/swagger](https://www.google.com/search?q=http://localhost:8080/swagger)

-----

## 🎮 Cliente de Consola (CLI)

El proyecto incluye una aplicación de terminal interactiva para probar la API.

**Nota:** Asegúrate de que el entorno Docker esté corriendo antes de iniciar el cliente.

```bash
# En una nueva terminal, desde la raíz del proyecto:
cd src/StarWars.ConsoleClient
dotnet run
```

## ✅ Funcionalidades (Requirements Checklist)

| Requisito | Estado | Detalles de Implementación |
| :--- | :---: | :--- |
| **Integración SWAPI** | ✅ | Consumo tipado con `HttpClientFactory`. |
| **Base de Datos** | ✅ | Persistencia de favoritos y logs en PostgreSQL. |
| **Docker** | ✅ | Orquestación completa con `docker-compose`. |
| **Favoritos** | ✅ | CRUD completo para gestión de personajes. |
| **Historial (Logs)** | ✅ | Middleware asíncrono para auditoría de tráfico. |
| **Caching (Bonus)** | ✅ | **Redis** distribuido para alta disponibilidad. |
| **Cliente Consola** | ✅ | UI rica e interactiva con `Spectre.Console`. |



-----


**Developed by Alfredo Bravo Cuero**
