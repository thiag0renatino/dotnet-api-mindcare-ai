FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["MindCareAi.csproj", "./"]
RUN dotnet restore "MindCareAi.csproj"

COPY . .
FROM build AS publish
RUN dotnet publish "MindCareAi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MindCareAi.dll"]
