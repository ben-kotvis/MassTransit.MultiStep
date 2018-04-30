FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 8082

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY MassTransit.MultiStep.sln ./
COPY MassTransit.MultiStep.CreditService/MassTransit.MultiStep.CreditService.csproj MassTransit.MultiStep.CreditService/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/MassTransit.MultiStep.CreditService
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app -r linux-x64

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MassTransit.MultiStep.CreditService.dll"]
