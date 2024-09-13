# Created by: Team india
    # Martin Edwin Schjødt Nielsen
# Build: docker build . -t martinjakobmsdo/skycaveplayerservice
# docker push martinjakobmsdo/skycave (with default tag "latest")
# Execute with fakeplayerStorage as configuration file: docker run -p 80:80 martinjakobmsdo/skycaveplayerservice
# Excute  with specific configurataion file: docker run -p 80:80 martinjakobmsdo/skycaveplayerservice dotnet SkycavePlayerService.api.dll Configuration:File=<configuration filename>.json

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

EXPOSE 80
COPY SkycavePlayerService.api/ SkycavePlayerService.api/
COPY SkycavePlayerService.Implementations.Repositories/ SkycavePlayerService.Implementations.Repositories/
COPY SkycavePlayerService.Exceptions/ SkycavePlayerService.Exceptions/
COPY SkycavePlayerService.Shared.Contracts/ SkycavePlayerService.Shared.Contracts/
COPY SkycavePlayerService.Shared.Models/ SkycavePlayerService.Shared.Models/
COPY SkycavePlayerService.Implementations.TestDoubles/ SkycavePlayerService.Implementations.TestDoubles/
COPY SkycavePlayerService.Implementations.Storage/ SkycavePlayerService.Implementations.Storage/

RUN dotnet restore "SkycavePlayerService.api/SkycavePlayerService.api.csproj"

WORKDIR "/src/SkycavePlayerService.api"
RUN dotnet build "SkycavePlayerService.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SkycavePlayerService.api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS "http://0.0.0.0:80"
CMD ["dotnet", "SkycavePlayerService.api.dll", "Configuration:File=FakePlayerStorage.json"]
