#!/bin/bash

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"
SRC_DIR="${SCRIPT_DIR}/../src"
IMAGE_BUILD_SCRIPT=${SCRIPT_DIR}/../docker-securitytools-build/docker.sh
IMAGE_NAME=ggolbik/securitytools
# get tag from version inside AssemblyInfo.cs
APP_VERSION="$(echo -n $(grep '<InformationalVersion>' ${SRC_DIR}/SecurityTools.csproj | sed -r 's/<InformationalVersion>(.*)<\/InformationalVersion>/\1/'))"
APP_VERSION=${APP_VERSION%%[[:space:]]}
APP_BUILDNUMBER="$(date +%s)"
APP_VERSION_PARTS=(${APP_VERSION//-/ })
if [ -z "${APP_VERSION_PARTS[1]}" ]; then
    APP_FULLVERSION=$(printf %s.%s "${APP_VERSION_PARTS[0]}" "${APP_BUILDNUMBER}")
else
    APP_FULLVERSION=$(printf %s.%s-%s "${APP_VERSION_PARTS[0]}" "${APP_BUILDNUMBER}" "${APP_VERSION_PARTS[1]}")
fi
IMAGE_TAG=${APP_VERSION}

BUILD_DIR="${SCRIPT_DIR}/../build"
ARCHITECTURE=$(uname -m)

echo "Build Version: ${APP_VERSION} (${APP_FULLVERSION})"

/bin/bash ${IMAGE_BUILD_SCRIPT}
EXIT_CODE=$?
if [[ ${EXIT_CODE} -ne 0 ]]; then
  exit ${EXIT_CODE}
fi

if [[ ! -d ${BUILD_DIR} ]]; then
  mkdir -p ${BUILD_DIR}
fi

# --force-rm: Remove intermediate containers after a build
/bin/bash -c "set -o pipefail \
  && cd ${SCRIPT_DIR} \
  && docker build \
  --force-rm=true \
  --target release \
  --tag ${IMAGE_NAME} \
  --tag ${IMAGE_NAME}:${IMAGE_TAG} \
  --build-arg LABEL_CREATED=$(date -u +'%Y-%m-%dT%H:%M:%SZ') \
  --build-arg LABEL_VERSION=${IMAGE_TAG} \
  --build-arg APP_FULLVERSION=${APP_FULLVERSION} \
  -f ./Dockerfile \
  ../"

DOTNET_VERSION=$(echo -n $(docker inspect ${IMAGE_NAME}:${IMAGE_TAG} | grep DOTNET_VERSION | head -1 | sed -r 's/"DOTNET_VERSION=(.*)",/\1/'))

docker save ${IMAGE_NAME} | gzip > "${BUILD_DIR}/ggolbik-securitytools-${APP_VERSION}_docker-${ARCHITECTURE}_self-contained-${DOTNET_VERSION}.tar.gz"
