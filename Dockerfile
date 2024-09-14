# Created by: Martin Edwin Schjødt Nielsen
# Build: docker build . -t lightmaze/player_record_service
# Push to Repository: docker push lightmaze/player_record_service (with default tag "latest")
# Execute with fakeplayerStorage as configuration file: docker run -p 80:80 lightmaze/player_record_service (Uses )
# Excute  with specific configurataion file: docker run -p 80:80 lightmaze/player_record_service dotnet PlayerRecordService.api.dll Configuration:File=<configuration filename>.json

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

EXPOSE 80
COPY PlayerRecordService.api/ PlayerRecordService.api/
COPY PlayerRecordService.Implementations.Repositories/ PlayerRecordService.Implementations.Repositories/
COPY PlayerRecordService.Exceptions/ PlayerRecordService.Exceptions/
COPY PlayerRecordService.Shared.Contracts/ PlayerRecordService.Shared.Contracts/
COPY PlayerRecordService.Shared.Models/ PlayerRecordService.Shared.Models/
COPY PlayerRecordService.Implementations.TestDoubles/ PlayerRecordService.Implementations.TestDoubles/
COPY PlayerRecordService.Implementations.Storage/ PlayerRecordService.Implementations.Storage/

RUN dotnet restore "PlayerRecordService.api/PlayerRecordService.api.csproj"

WORKDIR "/src/PlayerRecordService.api"
RUN dotnet build "PlayerRecordService.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PlayerRecordService.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS "http://0.0.0.0:80"
CMD ["dotnet", "PlayerRecordService.api.dll", "Configuration:File=FakePlayerStorage.json"]
