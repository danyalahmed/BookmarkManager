#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:2.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:2.1 AS build
WORKDIR /src
COPY ["Cw03/CW03.csproj", "Cw03/"]
RUN dotnet restore "Cw03/CW03.csproj"
COPY . .
WORKDIR "/src/Cw03"
RUN dotnet build "CW03.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CW03.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CW03.dll"]