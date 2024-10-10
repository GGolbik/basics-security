#!/bin/bash

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"

BUILD_DIR="${SCRIPT_DIR}/build"
TOOLCHAIN_DOTNET="$(which dotnet)"
TEST_DIR="${SCRIPT_DIR}/SecurityToolsTest"
TEST_OUT_DIR="${BUILD_DIR}/SecurityToolsTest"
REPORT_GENERATOR="$(which reportgenerator)"

echoerr() { echo "$@" 1>&2; }

main() {

  create_build_dir

  build_and_execute_tests
  local ret=$?

  create_coverage_report
  return ${ret}
}

create_build_dir() {
  if [[ ! -d ${BUILD_DIR} ]]; then
    echo "Create build directory: ${BUILD_DIR}"
    mkdir -p ${BUILD_DIR}
  fi
}

build_and_execute_tests() {
  echo "Build and execute tests"
  /bin/bash -c "set -o pipefail \
    && cd ${TEST_DIR} \
    && ${TOOLCHAIN_DOTNET} restore \
    && ${TOOLCHAIN_DOTNET} test --logger \"html;logfilename=result.html\" --results-directory ${TEST_OUT_DIR} --collect:\"XPlat Code Coverage\""
  return $?
}

create_coverage_report() {
  install_reportgenerator

  local coverage_file=$(find "${TEST_OUT_DIR}/" -name "coverage.cobertura.xml" -print -quit)
  local ret=$?
  if [[ ${ret} -eq 0 ]]; then
    mv "${coverage_file}" ${TEST_OUT_DIR}/coverage.cobertura.xml
    /bin/bash -c "set -o pipefail \
      && cd ${TEST_DIR} \
      && ${REPORT_GENERATOR} \"-reports:${TEST_OUT_DIR}/coverage.cobertura.xml\" \"-targetdir:${TEST_OUT_DIR}/coverage\" -reporttypes:HTML;"
  else
    echoerr "Coverage report has not been found. ERROR: ${ret}"
  fi
  return ${ret}
}

install_reportgenerator() {
  local tool_name="dotnet-reportgenerator-globaltool"
  local ret=$(dotnet tool list -g | grep -wc ${tool_name})
  if [[ ${ret} -eq 0 ]]; then
    echo "Installing ${tool_name}"
    dotnet tool install -g ${tool_name}
    source ~/.bashrc
  else
    echo "${tool_name} is already installed."
  fi
  REPORT_GENERATOR="$(which reportgenerator)"
}

# run script
main "$@"
EXIT_CODE=$?
if [[ ${EXIT_CODE} -eq 0 ]]; then
  echo "Test was successful"
else
  echoerr "Test failed with ERROR: ${EXIT_CODE}"
  exit ${EXIT_CODE}
fi