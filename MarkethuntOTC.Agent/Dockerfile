FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MarkethuntOTC.Agent/MarkethuntOTC.Agent.csproj", "MarkethuntOTC.Agent/"]
RUN dotnet restore "MarkethuntOTC.Agent/MarkethuntOTC.Agent.csproj"
COPY . .
WORKDIR "/src/MarkethuntOTC.Agent"
RUN dotnet build "MarkethuntOTC.Agent.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MarkethuntOTC.Agent.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MarkethuntOTC.Agent.dll"]
