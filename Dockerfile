FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ControleFinanceiro-Backend.csproj ./
RUN dotnet restore ./ControleFinanceiro-Backend.csproj

COPY . ./
RUN dotnet publish ./ControleFinanceiro-Backend.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish ./
USER app
ENTRYPOINT ["dotnet", "ControleFinanceiro-Backend.dll"]
