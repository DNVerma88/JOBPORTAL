# ── Build stage ───────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

# Override NuGet config to remove Windows-only fallback folders
COPY NuGet.Config ./

# Restore — copy project files first for layer caching
COPY src/JobPortal.Domain/JobPortal.Domain.csproj             src/JobPortal.Domain/
COPY src/JobPortal.Application/JobPortal.Application.csproj   src/JobPortal.Application/
COPY src/JobPortal.Infrastructure/JobPortal.Infrastructure.csproj src/JobPortal.Infrastructure/
COPY src/JobPortal.API/JobPortal.API.csproj                   src/JobPortal.API/
RUN dotnet restore src/JobPortal.API/JobPortal.API.csproj

# Copy source and publish
COPY src/ src/
RUN dotnet publish src/JobPortal.API/JobPortal.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Runtime stage ──────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

# Non-root user for security (OWASP A05)
RUN groupadd --system --gid 1001 appgroup \
 && useradd  --system --uid 1001 --gid appgroup --no-create-home appuser

COPY --from=build --chown=appuser:appgroup /app/publish .

# Logs directory
RUN mkdir -p /app/logs && chown appuser:appgroup /app/logs

USER appuser

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "JobPortal.API.dll"]
