FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ArispexBlacklistSystemBot/ArispexBlacklistSystemBot.csproj", "ArispexBlacklistSystemBot/"]
RUN dotnet restore "ArispexBlacklistSystemBot/ArispexBlacklistSystemBot.csproj"
COPY . .
WORKDIR "/src/ArispexBlacklistSystemBot"
RUN dotnet build "ArispexBlacklistSystemBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ArispexBlacklistSystemBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArispexBlacklistSystemBot.dll"]
