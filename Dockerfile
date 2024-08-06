FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY *.csproj ./

RUN dotnet restore

COPY . ./

RUN dotnet build -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
RUN apt update
RUN apt install -y procps
RUN apt install -y curl
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "izota-dummy.dll"]