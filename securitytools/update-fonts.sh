#!/bin/bash

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"

STYLES_DIR="${SCRIPT_DIR}/SecurityToolsWeb/src/styles"
ASSERTS_DIR="${SCRIPT_DIR}/SecurityToolsWeb/src/assets/fonts/"
FONTS_FILE="${SCRIPT_DIR}/SecurityToolsWeb/src/assets/fonts/.fonts"
ROBOTO_STYLE="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500&display=swap"
ROBOTO_FILE="${STYLES_DIR}/roboto.css"
MATERIAL_ICONS_STYLE="https://fonts.googleapis.com/icon?family=Material+Icons"
MATERIAL_ICONS_FILE="${STYLES_DIR}/material-icons.css"
MATERIAL_SYMBOLS_OUTLINED_STYLE="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined:opsz,wght,FILL,GRAD@24,400,0,0&icon_names=deployed_code"
MATERIAL_SYMBOLS_OUTLINED_FONT="https://fonts.gstatic.com/icon/font?kit=kJF1BvYX7BgnkSrUwT8OhrdQw4oELdPIeeII9v6oDMzByHX9rA6RzaxHMPdY43zj-jCxv3fzvRNU22ZXGJpEpjC_1v-p_4MrImHCIJIZrDCvHOejHdcfyVBAtqx9--ue8Mq0&skey=b8dc2088854b122f&v=v213"

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