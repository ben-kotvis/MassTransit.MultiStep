FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY MassTransit.MultiStep.sln ./
COPY MassTransit.MultiStep.Portal/MassTransit.MultiStep.Portal.csproj MassTransit.MultiStep.Portal/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/MassTransit.MultiStep.Portal
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app -r linux-x64

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MassTransit.MultiStep.Portal.dll"]
