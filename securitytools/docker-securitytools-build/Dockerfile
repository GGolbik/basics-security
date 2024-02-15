# Use dotnet as build container
FROM mcr.microsoft.com/dotnet/sdk:7.0

# Define arguments used in this dockerfile
ARG LABEL_CREATED
ARG LABEL_VERSION="1.0.0"

# Update package list and install dependencies
RUN /bin/bash -c set -o pipefail \
  && apt-get update \
  && apt-get install -y \
  curl \
  zip \
  build-essential

# Install Node.js and npm
RUN /bin/bash -c set -o pipefail \
  && curl -fsSL https://deb.nodesource.com/setup_18.x | bash - \
  && apt-get update \
  && apt-get install -y \
  nodejs

# Set labels - OCI Image Format https://www.opencontainers.org/
LABEL \
  # version of the packaged software
  org.opencontainers.image.version=${LABEL_VERSION} \
  # date and time on which the image was built (string, date-time as defined by RFC 3339).
  org.opencontainers.image.created=${LABEL_CREATED} \
  # Name of the distributing entity, organization or individual.
  org.opencontainers.image.vendor="GGolbik" \
  # Text description of the image.
  org.opencontainers.image.description=".NET Node.js build container."
