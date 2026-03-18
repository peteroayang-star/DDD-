FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/DddTemplate.Api/DddTemplate.Api.csproj", "src/DddTemplate.Api/"]
COPY ["src/DddTemplate.Application/DddTemplate.Application.csproj", "src/DddTemplate.Application/"]
COPY ["src/DddTemplate.Domain/DddTemplate.Domain.csproj", "src/DddTemplate.Domain/"]
COPY ["src/DddTemplate.Infrastructure.InMemory/DddTemplate.Infrastructure.InMemory.csproj", "src/DddTemplate.Infrastructure.InMemory/"]
RUN dotnet restore "src/DddTemplate.Api/DddTemplate.Api.csproj"
COPY . .
WORKDIR "/src/src/DddTemplate.Api"
RUN dotnet build "DddTemplate.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DddTemplate.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DddTemplate.Api.dll"]
