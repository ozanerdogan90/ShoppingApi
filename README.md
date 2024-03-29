# Shopping Api With Akka.net

> .Net Core 2.2, Akka.net , Docker

## Description
This project aims to create shopping cart and inventory management application via using akka.net actors.
Akka server is used as distributed command bus with multiple instance of actors.Shopping api reaches this actors by akka remote connection (address is configured in appsettings.json)
For each adding product and purchase actions, warehouse actor checks daily warehouse product limit and product stock limit and if the limits are excess server will return exception

## Features
##### Framework
- .Net Core
- Akka.Net
#####  Testing
- Xunit
- Fluent Assertions
## Requirements
- .Net Core >= 2.2
- Docker

## Running the API
### Development
To start the application in development mode, run:

```cmd
dotnet build
cd src\ShoppingApi
dotnet run
```
Application will be served on route: 
http://localhost:5000

To start the application in docker container:
```cmd
docker-compose up
```
Docker will spin up application, akka server will be running and also shopping api

To start the akka server in docker container:
```cmd
docker-compose -f docker-compose-akka-server.yml up
```
Docker will spin up akka server and shopping api can run seperately


## Swagger
Swagger documentation will be available on route: 
```bash
http://localhost:5000/swagger
```

### Testing
To run tests: 
```bash
dotnet test ./tests/ShoppingApi.AkkaServer.Tests/ShoppingApi.AkkaServer.Tests.csproj
```

### TO-DO List
- There is no storage layer. Akka is able to persist info but its not configure yet

### Feature List
- Return product details