FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY astorWorkBackEnd.sln ./
COPY astorWorkBackEnd/*.csproj astorWorkBackEnd/
COPY astorWorkBackEndUnitTest/*.csproj astorWorkBackEndUnitTest/
COPY astorWorkGateWay/*.csproj astorWorkGateWay/
COPY astorWorkDAO/*.csproj astorWorkDAO/
COPY astorWorkShared/*.csproj astorWorkShared/
COPY astorWorkUserManage/*.csproj astorWorkUserManage/
COPY astorWorkUserManageUnitTest/*.csproj astorWorkUserManageUnitTest/
RUN dotnet restore -nowarn:msb3202,nu1503
COPY . .
WORKDIR /src/astorWorkGateWay
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "astorWorkGateWay.dll"]
