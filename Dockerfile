FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . ./

WORKDIR /src/Platform.Catalog.API

RUN dotnet restore Platform.Catalog.API.csproj
RUN dotnet publish Platform.Catalog.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Platform.Catalog.API.dll"]
