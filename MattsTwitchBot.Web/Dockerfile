FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["MattsTwitchBot.Web/MattsTwitchBot.Web.csproj", "MattsTwitchBot.Web/"]
RUN dotnet restore "MattsTwitchBot.Web/MattsTwitchBot.Web.csproj"
COPY . .
WORKDIR "/src/MattsTwitchBot.Web"
RUN dotnet build "MattsTwitchBot.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "MattsTwitchBot.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MattsTwitchBot.Web.dll"]