FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["TheLiar.Api/TheLiar.Api.csproj", "TheLiar.Api/"]
RUN dotnet restore "TheLiar.Api/TheLiar.Api.csproj"
COPY . .
WORKDIR "/src/TheLiar.Api"
RUN dotnet build "TheLiar.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TheLiar.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TheLiar.Api.dll"]
