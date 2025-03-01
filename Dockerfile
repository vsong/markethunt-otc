FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -o /build

FROM mcr.microsoft.com/dotnet/runtime:8.0 as run
WORKDIR /app
COPY --from=build /build .
ENTRYPOINT ["dotnet", "MarkethuntOTC.Agent.dll"]
