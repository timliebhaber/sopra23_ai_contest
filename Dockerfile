FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ki.csproj", "./"]
RUN dotnet restore "ki.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "ki.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ki.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
ADD "/BotC_Ai/Schemas/config/boardConfig.schema.json" "/app/BotC_Ai/Schemas/config/boardConfig.schema.json"
ADD "/BotC_Ai/Schemas/config/gameConfig.schema.json" "/app/BotC_Ai/Schemas/config/gameConfig.schema.json"
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ki.dll"]
