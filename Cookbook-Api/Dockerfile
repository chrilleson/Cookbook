FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["./Cookbook.Api/Cookbook.Api.csproj", "./"]
COPY ["./Cookbook.Application/Cookbook.Application.csproj", "./"]
COPY ["./Cookbook.Common/Cookbook.Common.csproj", "./"]
COPY ["./Cookbook.Domain/Cookbook.Domain.csproj", "./"]
COPY ["./Cookbook.Infrastructure/Cookbook.Infrastructure.csproj", "./"]
COPY ["./Cookbook.Repositories/Cookbook.Repositories.csproj", "./"]
COPY Directory.Build.props ./
RUN dotnet restore "Cookbook.Api.csproj"
COPY . .
WORKDIR "/src/Cookbook.Api"
RUN dotnet build "Cookbook.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Cookbook.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS="https://+:443"
ENV ASPNETCORE_HTTPS_PORT=5001
ENV ASPNETCORE_ENVIRONMENT=Docker
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="Qwerty123!"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/https/localhost.pfx

ENTRYPOINT [ "dotnet", "Cookbook.Api.dll" ]