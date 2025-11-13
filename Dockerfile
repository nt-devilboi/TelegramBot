FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443



FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG CONN_STRING 

RUN dotnet tool install --global dotnet-ef --version 8.0.22
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet ef --version 
WORKDIR /src
RUN mkdir -p CookingBot
COPY ./CookingBot ./CookingBot
RUN dotnet restore "./CookingBot/CookingBot.csproj"


WORKDIR /src/CookingBot
RUN dotnet build "CookingBot.csproj" -c Release -o /app/build
RUN echo "Подключение к бд $CONN_STRING"
RUN dotnet ef database update --connection "$CONN_STRING"

FROM build AS publish
RUN dotnet publish "CookingBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CookingBot.dll"]
