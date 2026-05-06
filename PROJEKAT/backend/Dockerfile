# Stage 1: build + publish
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Kopiraj samo csproj fajlove prvo da Docker keširao restore layer
COPY src/PostRoute.Api/PostRoute.Api.csproj       src/PostRoute.Api/
COPY src/PostRoute.BLL/PostRoute.BLL.csproj       src/PostRoute.BLL/
COPY src/PostRoute.DAL/PostRoute.DAL.csproj       src/PostRoute.DAL/
COPY src/PostRoute.Domain/PostRoute.Domain.csproj src/PostRoute.Domain/
RUN dotnet restore src/PostRoute.Api/PostRoute.Api.csproj

# Kopiraj ostatak source-a i publishuj
COPY src/ src/
RUN dotnet publish src/PostRoute.Api/PostRoute.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Non-root korisnik radi sigurnosti
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

# Render dodjeljuje port kroz $PORT env varijablu. Lokalno fallback na 5000.
ENV ASPNETCORE_URLS=http://+:${PORT:-5000}
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "PostRoute.Api.dll"]
