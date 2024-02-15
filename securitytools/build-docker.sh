#!/bin/bash

SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"

IMAGE_NAME=mcr.microsoft.com/dotnet/sdk:7.0
BUILD_DIR="${SCRIPT_DIR}/build"

docker pull ${IMAGE_NAME}
EXIT_CODE=$?
if [[ ${EXIT_CODE} -ne 0 ]]; then
  exit ${EXIT_CODE}
fi

CONTAINER_ID=$(docker run --interactive --tty --detach ${IMAGE_NAME})

mkdir -p ${BUILD_DIR}

docker cp ${SCRIPT_DIR}/. ${CONTAINER_ID}:/app

docker exec \
  --interactive \
  --tty \
  ${CONTAINER_ID} rm -r /app/build

docker exec \
  --interactive \
  --tty \
  ${CONTAINER_ID} bash /app/build.sh

docker cp ${CONTAINER_ID}:/app/build/. ${BUILD_DIR}

docker stop ${CONTAINER_ID}

echo "Remove container:"
docker rm ${CONTAINER_ID}
