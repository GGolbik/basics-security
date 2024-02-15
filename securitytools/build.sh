#!/bin/bash

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"

# "Debug" or empty for release build
BUILD_TYPE="Release"
BUILD_DIR="${SCRIPT_DIR}/build"
SRC_DIR="${SCRIPT_DIR}/src"
TEST_DIR="${SCRIPT_DIR}/test"
TEST_OUT_DIR="${BUILD_DIR}/test"
TOOLCHAIN_DOTNET="$(which dotnet)"
TEST_SCRIPT=${SCRIPT_DIR}/test.sh
DEB_SCRIPT=${SCRIPT_DIR}/build-deb.sh
VERSION_NUMBER=""
INFORMATIONAL_VERSION=""
VERSION_NAME=""
RELEASE_DATE=$(date -u +%Y-%m-%dT%H:%M:%SZ)

echoerr() { echo "$@" 1>&2; }

main() {
  init_args "$@"

  set_version_name

  read_version

  read_informational_version

  create_build_dir

  execute_tests

  build_app

  package_app
  local ret=$?
  return ${ret}
}

init_args() {
  echo "ARGS: $@"
  local build_type="$1"
  if [ -n "${build_type}" ]; then
    BUILD_TYPE="${build_type}"
  fi
}

set_version_name() {
  if [ -z $VERSION_NAME ]; then
    which git
    local ret=$?
    if [[ ${ret} -eq 0 ]]; then
      /bin/bash -c "set -o pipefail \
        && cd ${SRC_DIR} \
        && git status"
      ret=$?
      if [[ ${ret} -eq 0 ]]; then
        local branch=$(/bin/bash -c "set -o pipefail && cd \"${SRC_DIR}\" && git rev-parse --abbrev-ref HEAD")
        local commit=$(/bin/bash -c "set -o pipefail && cd \"${SRC_DIR}\" && git rev-parse HEAD")
        if [ -z $(git diff -s) ]; then
          commit+=":"
          commit+=$(git diff -s | sha1sum | awk '{ print $1 }')
        fi;
        VERSION_NAME="${branch} ${commit}"
      fi
    fi
  fi
}

read_version() {
  # version is not provided by project.
  local version="$(echo -n $(grep '<Version>' ${SRC_DIR}/SecurityTools.csproj | sed -r 's/<Version>(.*)<\/Version>/\1/'))"
  version=${version%%[[:space:]]}
  # use unix time as build number
  local build="$(date +%s)"
  VERSION_NUMBER="${version}.${build}"
}

read_informational_version() {
  local informationalVersion="$(echo -n $(grep '<InformationalVersion>' ${SRC_DIR}/SecurityTools.csproj | sed -r 's/<InformationalVersion>(.*)<\/InformationalVersion>/\1/'))"
  informationalVersion=${informationalVersion%%[[:space:]]}
  if [ "${informationalVersion}" = "" ]; then
    # version is not provided by project.
    local version="$(echo -n $(grep '<Version>' ${SRC_DIR}/SecurityTools.csproj | sed -r 's/<Version>(.*)<\/Version>/\1/'))"
    version=${version%%[[:space:]]}
    # use unix time as build number
    local build="$(date +%s)"
    INFORMATIONAL_VERSION="${version}.${build}"
  else
    # version is defined by project.
    INFORMATIONAL_VERSION=$informationalVersion
  fi
}

create_build_dir() {
  if [[ ! -d ${BUILD_DIR} ]]; then
    echo "Create build directory: ${BUILD_DIR}"
    mkdir -p ${BUILD_DIR}
  fi
}

execute_tests() {
  bash "${TEST_SCRIPT}"
}

build_app() {
  echo "Build app: ${INFORMATIONAL_VERSION}"
  /bin/bash -c "set -o pipefail \
    && cd ${SRC_DIR} \
    && ${TOOLCHAIN_DOTNET} restore \
    && ${TOOLCHAIN_DOTNET} publish -c ${BUILD_TYPE} -p:InformationalVersion=${INFORMATIONAL_VERSION} -p:ReleaseDate=${RELEASE_DATE} -p:VersionName=\"${VERSION_NAME}\" -o ${BUILD_DIR}/${BUILD_TYPE} \
    && ${TOOLCHAIN_DOTNET} publish -c ${BUILD_TYPE} -p:InformationalVersion=${INFORMATIONAL_VERSION} -p:ReleaseDate=${RELEASE_DATE} -p:VersionName=\"${VERSION_NAME}\" -o ${BUILD_DIR}/win10-x64-self-contained/ --runtime win10-x64 --self-contained true \
    && ${TOOLCHAIN_DOTNET} publish -c ${BUILD_TYPE} -p:InformationalVersion=${INFORMATIONAL_VERSION} -p:ReleaseDate=${RELEASE_DATE} -p:VersionName=\"${VERSION_NAME}\" -o ${BUILD_DIR}/linux-x64-self-contained/ --runtime linux-x64 --self-contained true"
}

install_zip() {
  local path=$(which zip)
  if [ "${path}" = "" ]; then
    echo "Install zip"
    sudo apt-get install zip
  fi
}

package_app() {
  install_zip

  echo "Package app"
  /bin/bash -c "set -o pipefail \
    && cd ${BUILD_DIR} \
    && zip -r \"ggolbik-securitytools_${VERSION_NUMBER}_win10-x86-64.zip\" \"win10-x64-self-contained\" \
    && tar -czvf \"ggolbik-securitytools_${VERSION_NUMBER}_linux-x86-64.tar.gz\" \"linux-x64-self-contained/\""

  /bin/bash "${DEB_SCRIPT}"
}

# run script
main "$@"
EXIT_CODE=$?
if [[ ${EXIT_CODE} -eq 0 ]]; then
  echo "Build was successful"
else
  echoerr "Build failed with ERROR: ${EXIT_CODE}"
  exit ${EXIT_CODE}
fi
