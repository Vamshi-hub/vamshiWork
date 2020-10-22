FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY . .
WORKDIR /src/astorWorkBackgroundService
RUN dotnet publish -c Release -o /app

FROM base AS final

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        zlib1g \
        fontconfig \
        libfreetype6 \
        libx11-6 \
        libxext6 \
        libxrender1 \
    && curl -o /usr/lib/libwkhtmltox.so \
        --location \
        https://github.com/rdvojmoc/DinkToPdf/raw/v1.0.8/v0.12.4/64%20bit/libwkhtmltox.so

WORKDIR /app
COPY --from=build /app .
##RUN export LD_LIBRARY_PATH=/app:$LD_LIBRARY_PATH
ENTRYPOINT ["dotnet", "astorWorkBackgroundService.dll"]

