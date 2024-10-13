#!/bin/bash

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"

STYLES_DIR="${SCRIPT_DIR}/SecurityToolsWeb/src/styles"
ASSERTS_DIR="${SCRIPT_DIR}/SecurityToolsWeb/src/assets/fonts/"
FONTS_FILE="${SCRIPT_DIR}/SecurityToolsWeb/src/assets/fonts/.fonts"
ROBOTO_STYLE="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500&display=swap"
ROBOTO_FILE="${STYLES_DIR}/roboto.css"
MATERIAL_ICONS_STYLE="https://fonts.googleapis.com/icon?family=Material+Icons"
MATERIAL_ICONS_FILE="${STYLES_DIR}/material-icons.css"

rm -f "${ROBOTO_FILE}"
rm -f "${MATERIAL_ICONS_FILE}"

rm -r -f "${ASSERTS_DIR}"
mkdir -p ${ASSERTS_DIR}

# install tool license
/bin/bash -c "set -o pipefail \
  && cd ${ASSERTS_DIR} \
  && wget -O \"${ROBOTO_FILE}\" \"${ROBOTO_STYLE}\" \
  && wget -O  \"${MATERIAL_ICONS_FILE}\" \"${MATERIAL_ICONS_STYLE}\" \
  && grep url ${ROBOTO_FILE} | sed -r 's/.*url\((http[^!\)]*)\).*/\1/' > \"${FONTS_FILE}\" \
  && wget --directory-prefix \"${ASSERTS_DIR}\" -i \"${FONTS_FILE}\" \
  && rm \"${FONTS_FILE}\" \
  && grep url ${MATERIAL_ICONS_FILE} | sed -r 's/.*url\((http[^!\)]*)\).*/\1/' > \"${FONTS_FILE}\" \
  && wget --directory-prefix \"${ASSERTS_DIR}\" -i \"${FONTS_FILE}\" \
  && rm \"${FONTS_FILE}\""