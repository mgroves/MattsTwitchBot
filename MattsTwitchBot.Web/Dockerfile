FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["MattsTwitchBot.Web/MattsTwitchBot.Web.csproj", "MattsTwitchBot.Web/"]
COPY ["MattsTwitchBot.Core/MattsTwitchBot.Core.csproj", "MattsTwitchBot.Core/"]
RUN dotnet restore "MattsTwitchBot.Web/MattsTwitchBot.Web.csproj"
COPY . .
WORKDIR "/src/MattsTwitchBot.Web"
RUN dotnet build "MattsTwitchBot.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MattsTwitchBot.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MattsTwitchBot.Web.dll"]
