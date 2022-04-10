#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["tests/TestBackend/IdentityServer.csproj", "tests/TestBackend/"]
COPY ["Providers/Twilio/TwilioProviders.csproj", "Providers/Twilio/"]
COPY ["IdentityServer4.Endpoints/IdentityServer4.Anonymous.csproj", "IdentityServer4.Endpoints/"]
COPY ["IdentityModel.PhoneAuthorizationEndpoint/IdentityModel.Anonymous.csproj", "IdentityModel.PhoneAuthorizationEndpoint/"]
RUN dotnet restore "tests/TestBackend/IdentityServer.csproj"
COPY . .
WORKDIR "/src/tests/TestBackend"
RUN dotnet build "IdentityServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IdentityServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IdentityServer.dll"]
