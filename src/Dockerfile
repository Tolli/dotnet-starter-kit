# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files for restore (including transitive dependencies)
COPY ["Directory.Packages.props","."]
COPY ["Directory.Build.props","."]
COPY ["api/server/Server.csproj", "api/server/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["aspire/service-defaults/ServiceDefaults.csproj", "aspire/service-defaults/"]
COPY ["api/framework/Core/Core.csproj", "api/framework/Core/"]
COPY ["api/framework/Infrastructure/Infrastructure.csproj", "api/framework/Infrastructure/"]
COPY ["api/migrations/MSSQL/MSSQL.csproj", "api/migrations/MSSQL/"]
COPY ["api/migrations/PostgreSQL/PostgreSQL.csproj", "api/migrations/PostgreSQL/"]
COPY ["api/modules/Booking/Booking.Infrastructure/Booking.Infrastructure.csproj", "api/modules/Booking/Booking.Infrastructure/"]
COPY ["api/modules/Booking/Booking.Application/Booking.Application.csproj", "api/modules/Booking/Booking.Application/"]
COPY ["api/modules/Booking/Booking.Domain/Booking.Domain.csproj", "api/modules/Booking/Booking.Domain/"]
COPY ["api/modules/Catalog/Catalog.Infrastructure/Catalog.Infrastructure.csproj", "api/modules/Catalog/Catalog.Infrastructure/"]
COPY ["api/modules/Catalog/Catalog.Application/Catalog.Application.csproj", "api/modules/Catalog/Catalog.Application/"]
COPY ["api/modules/Catalog/Catalog.Domain/Catalog.Domain.csproj", "api/modules/Catalog/Catalog.Domain/"]
COPY ["api/modules/Todo/Todo.csproj", "api/modules/Todo/"]

# Restore dependencies
RUN dotnet restore "api/server/Server.csproj" -p:TargetFramework=net9.0

# Copy all source code
COPY Directory.Build.props .
COPY Directory.Packages.props .
COPY Shared/ Shared/
COPY aspire/service-defaults/ aspire/service-defaults/
COPY api/ api/

# Build and publish
WORKDIR "/src/api/server"
RUN dotnet publish "Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "FSH.Starter.WebApi.Host.dll"]