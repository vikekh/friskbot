FROM mcr.microsoft.com/dotnet/core/runtime:2.2-stretch-slim AS base
WORKDIR /app


FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["src/FriskBot.Cli/FriskBot.Cli.csproj", "src/FriskBot.Cli/"]
COPY ["src/SuperHappyScript/SuperHappyScript.csproj", "src/SuperHappyScript/"]
RUN dotnet restore "src/FriskBot.Cli/FriskBot.Cli.csproj"
COPY . .
WORKDIR "/src/src/FriskBot.Cli"
RUN dotnet build "FriskBot.Cli.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FriskBot.Cli.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FriskBot.Cli.dll"]
