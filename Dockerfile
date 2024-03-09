#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["BuJoProApi/BuJoProApi.csproj", "BuJoProApi/"]
RUN dotnet restore "BuJoProApi/BuJoProApi.csproj"
COPY . .
WORKDIR "/src/BuJoProApi"
RUN dotnet build "BuJoProApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BuJoProApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN apt-get update && apt-get install -y texlive
RUN apt-get install texlive-pictures -y
#
ENTRYPOINT ["dotnet", "BuJoProApi.dll"]