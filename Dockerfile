# Base stage for runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy Solution and Projects for Restore Caching
COPY ["Misan.Backend.sln", "."]
COPY ["src/Misan.Bootstrapper/Misan.Bootstrapper.csproj", "src/Misan.Bootstrapper/"]
COPY ["src/Misan.Shared.Kernel/Misan.Shared.Kernel.csproj", "src/Misan.Shared.Kernel/"]
COPY ["src/Misan.Modules.Identity/Misan.Modules.Identity.csproj", "src/Misan.Modules.Identity/"]
COPY ["src/Misan.Modules.Profiles/Misan.Modules.Profiles.csproj", "src/Misan.Modules.Profiles/"]
COPY ["src/Misan.Modules.Practitioner/Misan.Modules.Practitioner.csproj", "src/Misan.Modules.Practitioner/"]
COPY ["src/Misan.Modules.Clinical/Misan.Modules.Clinical.csproj", "src/Misan.Modules.Clinical/"]
COPY ["src/Misan.Modules.Store/Misan.Modules.Store.csproj", "src/Misan.Modules.Store/"]
COPY ["src/Misan.Modules.Intelligence/Misan.Modules.Intelligence.csproj", "src/Misan.Modules.Intelligence/"]

# Restore Dependencies
RUN dotnet restore "./Misan.Backend.sln"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/src/Misan.Bootstrapper"
RUN dotnet build "./Misan.Bootstrapper.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Misan.Bootstrapper.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Misan.Bootstrapper.dll"]
