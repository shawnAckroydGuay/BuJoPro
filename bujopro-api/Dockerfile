#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["bujopro-api/bujopro-api.csproj", "bujopro-api/"]
RUN dotnet restore "bujopro-api/bujopro-api.csproj"
COPY . .
WORKDIR "/src/bujopro-api"
RUN dotnet build "bujopro-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "bujopro-api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN sudo apt-get update && apt-get install -y texlive
#RUN sudo apt-get install texlive-pictures
#
ENTRYPOINT ["dotnet", "bujopro-api.dll"]