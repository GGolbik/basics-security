#!/bin/bash

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" >/dev/null 2>&1 && pwd)"

sudo cp ${SCRIPT_DIR}/ggolbik-securitytools.service /etc/systemd/system/ggolbik-securitytools.service
sudo systemctl daemon-reload
systemctl status ggolbik-securitytools
sudo systemctl start ggolbik-securitytools
sudo systemctl enable ggolbik-securitytools
systemctl status ggolbik-securitytools
