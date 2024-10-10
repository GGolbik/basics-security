#!/bin/bash

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"

# "Debug" or empty for release build
BUILD_TYPE="Release"
BUILD_DIR="${SCRIPT_DIR}/build"
SRC_DIR="${SCRIPT_DIR}/SecurityToolsApp"
WWWROOT_DIR="${SRC_DIR}/wwwroot"
TOOLCHAIN_DOTNET="$(which dotnet)"
TEST_SCRIPT=${SCRIPT_DIR}/test.sh
DEB_SCRIPT=${SCRIPT_DIR}/build-deb.sh
VERSION_NUMBER=""
VERSION_NUMBER_FULL=""
INFORMATIONAL_VERSION=""
# use unix time as build number
BUILD_NUMBER="$(date +%s)"
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

  build_3rd_party_license

  build_app

  build_electron

  package_app

  local ret=$?
  return ${ret}
}

init_args() {
  echo "ARGS: $@"
  local build_type=""
  local build_number=""
  # getopts OptionString Name [ Argument ...]
  # If a character in OptionString is followed by a : (colon), that option is expected to have an argument.
  while getopts t:b: opt
  do
    case $opt in
        t) build_type=$OPTARG;;
        b) build_number=$OPTARG;;
        ?) 
    esac
  done

  if [ -n "${build_type}" ]; then
    BUILD_TYPE="${build_type}"
  fi
  if [ -n "${build_number}" ]; then
    BUILD_NUMBER="${build_number}"
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
  VERSION_NUMBER="$(echo -n $(grep '<Version>' ${SRC_DIR}/SecurityTools.csproj | sed -r 's/<Version>(.*)<\/Version>/\1/'))"
  VERSION_NUMBER=${VERSION_NUMBER%%[[:space:]]}
  VERSION_NUMBER_FULL="${VERSION_NUMBER}.${BUILD_NUMBER}"
}

read_informational_version() {
  local informationalVersion="$(echo -n $(grep '<InformationalVersion>' ${SRC_DIR}/SecurityTools.csproj | sed -r 's/<InformationalVersion>(.*)<\/InformationalVersion>/\1/'))"
  informationalVersion=${informationalVersion%%[[:space:]]}
  if [ "${informationalVersion}" = "" ]; then
    # version is not provided by project.
    INFORMATIONAL_VERSION="${VERSION_NUMBER_FULL}"
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
    && ${TOOLCHAIN_DOTNET} publish -c ${BUILD_TYPE} -p:InformationalVersion=\"${INFORMATIONAL_VERSION}\" -p:ReleaseDate=\"${RELEASE_DATE}\" -p:VersionName=\"${VERSION_NAME}\" -o ${BUILD_DIR}/${BUILD_TYPE} \
    && ${TOOLCHAIN_DOTNET} publish -c ${BUILD_TYPE} -p:InformationalVersion=\"${INFORMATIONAL_VERSION}\" -p:ReleaseDate=\"${RELEASE_DATE}\" -p:VersionName=\"${VERSION_NAME}\" -o ${BUILD_DIR}/win-x64-self-contained/ --runtime win-x64 --self-contained true \
    && ${TOOLCHAIN_DOTNET} publish -c ${BUILD_TYPE} -p:InformationalVersion=\"${INFORMATIONAL_VERSION}\" -p:ReleaseDate=\"${RELEASE_DATE}\" -p:VersionName=\"${VERSION_NAME}\" -o ${BUILD_DIR}/linux-x64-self-contained/ --runtime linux-x64 --self-contained true"
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
    && cd \"${BUILD_DIR}\" \
    && ln -s \"win-x64-self-contained\" "securitytools" \
    && zip -r \"ggolbik-securitytools_${VERSION_NUMBER_FULL}_win-x86-64.zip\" \"securitytools\" \
    && rm "securitytools" \
    && ln -s \"win-x64-self-contained\" "securitytools" \
    && tar -czvf \"ggolbik-securitytools_${VERSION_NUMBER_FULL}_linux-x86-64.tar.gz\" \"securitytools\""

  /bin/bash "${DEB_SCRIPT}" -b "${BUILD_NUMBER}"

  local electron="${BUILD_DIR}/Electron"
  if [ -d "$electron" ]; then
    /bin/bash -c "set -o pipefail \
      && cd \"${electron}\" \
      && ln -s \"win-unpacked\" "securitytools" \
      && zip -r \"../ggolbik-securitytools_${VERSION_NUMBER_FULL}_app-win-x86-64.zip\" \"securitytools\" \
      && rm "securitytools" \
      && ln -s \"linux-unpacked\" "securitytools" \
      && tar -czvf \"../ggolbik-securitytools_${VERSION_NUMBER_FULL}_app-linux-x86-64.tar.gz\" \"securitytools\""
  fi
}

build_electron() {
  dotnet tool list -g ElectronNET.CLI
  local found=$?
  if [[ ${EXIT_CODE} -eq 0 ]]; then
    echo "Found ElectronNET.CLI"
  else
    echoerr "ElectronNET.CLI not found! Execute: dotnet tool install ElectronNET.CLI -g"
    return
  fi
  which wine
  found=$?
  if [[ ${EXIT_CODE} -eq 0 ]]; then
    echo "Found wine"
  else
    echoerr "wine not found! Execute: sudo apt install wine"
    return
  fi
  # replace white spaces in VERSION_NAME values
  local versionName=$(echo ${VERSION_NAME}| sed "s/ /\\\ /g")
  /bin/bash -c "set -o pipefail \
    && cd ${SRC_DIR} \
    && electronize build /target win /Version ${VERSION_NUMBER} /p:InformationalVersion=${INFORMATIONAL_VERSION} /p:ReleaseDate=${RELEASE_DATE} /p:VersionName=\"${versionName}\" \
    && electronize build /target linux /Version ${VERSION_NUMBER} /p:InformationalVersion=${INFORMATIONAL_VERSION} /p:ReleaseDate=${RELEASE_DATE} /p:VersionName=\"${versionName}\""
}

build_3rd_party_license() {
  dotnet tool list -g nuget-license
  local found=$?
  if [[ ${EXIT_CODE} -eq 0 ]]; then
    echo "Found nuget-license"
  else
    echoerr "nuget-license not found! Execute: dotnet tool install nuget-license -g"
    return
  fi
  # create license
  /bin/bash -c "set -o pipefail \
    && cd ${SRC_DIR} \
    && nuget-license -i ${SRC_DIR}/SecurityTools.csproj -o Table > ${WWWROOT_DIR}/licenses.txt"
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
