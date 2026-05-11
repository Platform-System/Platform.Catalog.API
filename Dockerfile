FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["main_repo/Platform.Catalog.API.csproj", "Platform.Catalog.API/"]
COPY ["Platform.Api/Platform.Api.csproj", "Platform.Api/"]
COPY ["Platform.Application/Platform.Application.csproj", "Platform.Application/"]
COPY ["Platform.Domain/Platform.Domain.csproj", "Platform.Domain/"]
COPY ["Platform.BuildingBlocks/Platform.BuildingBlocks.csproj", "Platform.BuildingBlocks/"]
COPY ["Platform.Contracts/Platform.Contracts.csproj", "Platform.Contracts/"]
COPY ["Platform.Infrastructure/Platform.Infrastructure.csproj", "Platform.Infrastructure/"]
COPY ["Platform.SharedKernel/Platform.SharedKernel.csproj", "Platform.SharedKernel/"]
COPY ["Platform.SystemContext/Platform.SystemContext.csproj", "Platform.SystemContext/"]

RUN dotnet restore "Platform.Catalog.API/Platform.Catalog.API.csproj"

COPY ["main_repo/", "Platform.Catalog.API/"]
COPY ["Platform.Api/", "Platform.Api/"]
COPY ["Platform.Application/", "Platform.Application/"]
COPY ["Platform.Domain/", "Platform.Domain/"]
COPY ["Platform.BuildingBlocks/", "Platform.BuildingBlocks/"]
COPY ["Platform.Contracts/", "Platform.Contracts/"]
COPY ["Platform.Infrastructure/", "Platform.Infrastructure/"]
COPY ["Platform.SharedKernel/", "Platform.SharedKernel/"]
COPY ["Platform.SystemContext/", "Platform.SystemContext/"]

WORKDIR "/src/Platform.Catalog.API"
RUN dotnet publish "Platform.Catalog.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Platform.Catalog.API.dll"]
