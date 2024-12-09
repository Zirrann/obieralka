# Etap 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Kopiowanie pliku projektu
COPY *.csproj ./

# Przywracanie zależności
RUN dotnet restore

# Kopiowanie reszty plików
COPY . .

# Publikowanie aplikacji
RUN dotnet publish -c Release -o out

# Etap 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Kopiowanie opublikowanej aplikacji z etapu build
COPY --from=build /app/out .

# Ustawienie punktu wejścia aplikacji
ENTRYPOINT ["dotnet", "Shop.DB.dll"]
