#!/bin/bash

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"

SRC_DIR="${SCRIPT_DIR}/src"
BUILD_DIR="${SCRIPT_DIR}/build"
DEB_DIR="${SCRIPT_DIR}/deb"
DEB_BUILD_DIR="${SCRIPT_DIR}/build/deb"
SERVICE_FILE="${SRC_DIR}/Targets/Linux/ggolbik-securitytools.service"
VERSION_NUMBER=""
BUILD_NUMBER="$(date +%s)"
CONTENT_DIR="linux-x64-self-contained"
DEB_NAME=""

echoerr() { echo "$@" 1>&2; }

main() {
  init_args "$@"

  read_version

  create_build_dir

  set_deb_name

  clear_and_prepare_build

  remove_comment_lines

  set_file_permissions

  build_package
}

init_args() {
  echo "ARGS: $@"
  local content_dir=""
  local build_number=""
  # getopts OptionString Name [ Argument ...]
  # If a character in OptionString is followed by a : (colon), that option is expected to have an argument.
  while getopts c:b: opt
  do
    case $opt in
        c) content_dir=$OPTARG;;
        b) build_number=$OPTARG;;
        ?) 
    esac
  done

  if [ -n "${build_number}" ]; then
    BUILD_NUMBER="${build_number}"
  fi
  if [ -n "${content_dir}" ]; then
    CONTENT_DIR="${content_dir}"
  fi
}

read_version() {
  # version is not provided by project.
  local version="$(echo -n $(grep '<Version>' ${SRC_DIR}/SecurityTools.csproj | sed -r 's/<Version>(.*)<\/Version>/\1/'))"
  version=${version%%[[:space:]]}
  VERSION_NUMBER="${version}.${BUILD_NUMBER}"
}

create_build_dir() {
  if [[ ! -d ${BUILD_DIR} ]]; then
    echo "Create build directory: ${BUILD_DIR}"
    mkdir -p ${BUILD_DIR}
  fi
}

set_deb_name() {
  # The Debian binary package file names conform to the following convention:
  # <foo>_<VersionNumber>-<DebianRevisionNumber>_<DebianArchitecture>.deb
  DEB_NAME="ggolbik-securitytools_${VERSION_NUMBER}_amd64"
}

clear_and_prepare_build() {
  /bin/bash -c "set -o pipefail \
    && rm --force --recursive ${DEB_BUILD_DIR} \
    && mkdir -p ${DEB_BUILD_DIR}/${DEB_NAME}/opt/ggolbik/securitytools \
    && cp -r ${DEB_DIR}/* ${DEB_BUILD_DIR}/${DEB_NAME} \
    && cp -r ${BUILD_DIR}/${CONTENT_DIR}/* ${DEB_BUILD_DIR}/${DEB_NAME}/opt/ggolbik/securitytools \
    && mkdir -p ${DEB_BUILD_DIR}/${DEB_NAME}/lib/systemd/system \
    && cp ${SERVICE_FILE} ${DEB_BUILD_DIR}/${DEB_NAME}/lib/systemd/system/ \
    && mkdir -p ${DEB_BUILD_DIR}/${DEB_NAME}/etc/opt/ggolbik/securitytools/appsettings.d/"
}

remove_comment_lines() {
  /bin/bash -c "set -o pipefail \
    && cd ${DEB_BUILD_DIR} \
    && sed '/^#/ d' < ./${DEB_NAME}/DEBIAN/control > ./${DEB_NAME}/DEBIAN/control.temp \
    && mv ./${DEB_NAME}/DEBIAN/control.temp ./${DEB_NAME}/DEBIAN/control \
    && sed -i '2s/.*/Version: ${VERSION_NUMBER}/' ./${DEB_NAME}/DEBIAN/control"
}

set_file_permissions() {
  chmod 0755 ${DEB_BUILD_DIR}/${DEB_NAME}/DEBIAN/postinst
  chmod 0755 ${DEB_BUILD_DIR}/${DEB_NAME}/DEBIAN/postrm
  chmod 0755 ${DEB_BUILD_DIR}/${DEB_NAME}/DEBIAN/preinst
  chmod 0755 ${DEB_BUILD_DIR}/${DEB_NAME}/DEBIAN/prerm
}

build_package() {
  /bin/bash -c "set -o pipefail \
    && cd ${DEB_BUILD_DIR} \
    && dpkg-deb --build ${DEB_NAME}"

  mv ${DEB_BUILD_DIR}/${DEB_NAME}.deb ${BUILD_DIR}/
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
