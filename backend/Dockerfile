# build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia sói o arquivo de projeto e busca dependências
COPY ["ContactApi.csproj", "./"]
RUN dotnet restore "ContactApi.csproj"

# Copia e compila
COPY . .
RUN dotnet publish "ContactApi.csproj" -c Release -o /app/publish

# Etapa final: imagem leve de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "ContactApi.dll"]
