FROM microsoft/dotnet:3.1-sdk AS build
WORKDIR /app

COPY LambdaUI.csproj .
RUN dotnet restore LambdaUI.csproj

COPY . .
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:3.1-runtime AS runtime
WORKDIR /app
COPY --from=build /app/out ./

ENTRYPOINT ["dotnet", "LambdaUI.dll"]
