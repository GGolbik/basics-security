#!/bin/bash

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"

OUT_DIR="${SCRIPT_DIR}/tmp"

if [[ ! -d ${BUILD_DIR} ]]; then
  mkdir -p ${BUILD_DIR}
fi

/bin/bash -c "set -o pipefail \
  && cd ${SCRIPT_DIR} \
  && docker run -itd \
  -p 8000:80 \
  -p 8443:443 \
  --name securitytools ggolbik/securitytools"