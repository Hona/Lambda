FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /app

COPY QueryMaster/QueryMaster.csproj QueryMaster/
RUN dotnet restore QueryMaster/QueryMaster.csproj

COPY LambdaUI/LambdaUI.csproj LambdaUI/
RUN dotnet restore LambdaUI/LambdaUI.csproj

COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/runtime:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "LambdaUI.dll"]
