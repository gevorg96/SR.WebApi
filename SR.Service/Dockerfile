#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SR.Service/SR.Service.csproj", "SR.Service/"]
COPY ["SR.Http/SR.Http.csproj", "SR.Http/"]
COPY ["SR.Domain/SR.Domain.csproj", "SR.Domain/"]
COPY ["SR.Application/SR.Application.csproj", "SR.Application/"]
COPY ["SR.Infrastructure/SR.Infrastructure.csproj", "SR.Infrastructure/"]
RUN dotnet restore "SR.Service/SR.Service.csproj"
COPY . .
WORKDIR "/src/SR.Service"
RUN dotnet build "SR.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SR.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SR.Service.dll"]