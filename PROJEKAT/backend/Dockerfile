# ─── Stage 1: Build ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files first — Docker caches this layer and skips restore
# on subsequent builds if .csproj files haven't changed.
COPY src/PostRoute.Api/PostRoute.Api.csproj       src/PostRoute.Api/
COPY src/PostRoute.BLL/PostRoute.BLL.csproj       src/PostRoute.BLL/
COPY src/PostRoute.DAL/PostRoute.DAL.csproj       src/PostRoute.DAL/
COPY src/PostRoute.Domain/PostRoute.Domain.csproj src/PostRoute.Domain/

RUN dotnet restore src/PostRoute.Api/PostRoute.Api.csproj

# Copy the rest of the source and publish
COPY src/ src/
RUN dotnet publish src/PostRoute.Api/PostRoute.Api.csproj \
    --configuration Release \
    --no-restore \
    --output /app/publish

# ─── Stage 2: Runtime ────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Run as non-root for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

COPY --from=build /app/publish .

# Render dodjeljuje dinamički $PORT. Lokalno fallback na 5000.
# ASPNETCORE_URLS koristi shell expansion pa mora kroz sh -c entrypoint.
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 5000
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-5000} dotnet PostRoute.Api.dll"]
