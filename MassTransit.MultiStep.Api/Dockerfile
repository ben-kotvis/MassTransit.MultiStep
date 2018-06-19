FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY MassTransit.MultiStep.Api/MassTransit.MultiStep.Api.csproj MassTransit.MultiStep.Api/
COPY MassTransit.MultiStep.CommonBusSetup/MassTransit.MultiStep.CommonBusSetup.csproj MassTransit.MultiStep.CommonBusSetup/
COPY MassTransit.MultiStep.Common/MassTransit.MultiStep.Common.csproj MassTransit.MultiStep.Common/
RUN dotnet restore MassTransit.MultiStep.Api/MassTransit.MultiStep.Api.csproj
COPY . .
WORKDIR /src/MassTransit.MultiStep.Api
RUN dotnet build MassTransit.MultiStep.Api.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish MassTransit.MultiStep.Api.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MassTransit.MultiStep.Api.dll"]
