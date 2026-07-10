FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MGM.MediaServices.sln", "."]
COPY ["src/MGM.MediaServices.Api/MGM.MediaServices.Api.csproj", "src/MGM.MediaServices.Api/"]
COPY ["src/MGM.MediaServices.Core/MGM.MediaServices.Core.csproj", "src/MGM.MediaServices.Core/"]
COPY ["src/MGM.MediaServices.Worker/MGM.MediaServices.Worker.csproj", "src/MGM.MediaServices.Worker/"]
COPY ["src/MGM.MediaServices.Tests/MGM.MediaServices.Tests.csproj", "src/MGM.MediaServices.Tests/"]
RUN dotnet restore "MGM.MediaServices.sln"

COPY . .
WORKDIR "/src/src/MGM.MediaServices.Api"
RUN dotnet publish "MGM.MediaServices.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MGM.MediaServices.Api.dll"]
