FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["ShoppingApi/ShoppingApi.csproj", "ShoppingApi/"]
RUN dotnet restore "ShoppingApi/ShoppingApi.csproj"
COPY . .
WORKDIR "/src/ShoppingApi"
RUN dotnet build "ShoppingApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "ShoppingApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ShoppingApi.dll"]