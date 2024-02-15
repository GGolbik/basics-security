FROM ggolbik/securitytools-build:1.0 AS build

# Define arguments used in this dockerfile
ARG LABEL_CREATED
ARG LABEL_VERSION "1.0.0"
ARG APP_FULLVERSION "1.0.0"

WORKDIR /app/src

# copy csproj and restore as distinct layers
COPY src/*.csproj ./
RUN dotnet restore

# copy everything else and build app
COPY ./src ./

RUN dotnet new tool-manifest
RUN dotnet tool install dotnet-project-licenses
RUN dotnet dotnet-project-licenses -i ./ -o --outfile wwwroot/licenses.txt

RUN dotnet publish -c Release -o out -p:InformationalVersion=${APP_FULLVERSION}

######################################################
### Move from build container to runtime container ###
######################################################


# Use ASP.NET Core Runtime as release container - https://hub.docker.com/_/microsoft-dotnet-aspnet
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS release

# Set labels - OCI Image Format https://www.opencontainers.org/
LABEL \
  # version of the packaged software
  org.opencontainers.image.version=${LABEL_VERSION} \
  # date and time on which the image was built (string, date-time as defined by RFC 3339).
  org.opencontainers.image.created=${LABEL_CREATED} \
  # Name of the distributing entity, organization or individual.
  org.opencontainers.image.vendor="GGolbik" \
  # Text description of the image.
  org.opencontainers.image.description=""

WORKDIR /app
COPY --from=build /app/src/out ./

ENV ASPNETCORE_URLS=http://+:80;https://+:443
ENV SecurityTools__ApplicationDataDirectory=/var/opt/ggolbik/securitytools

EXPOSE 80
EXPOSE 443

CMD ["./SecurityTools"]
