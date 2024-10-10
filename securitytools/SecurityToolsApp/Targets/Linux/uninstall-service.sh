#!/bin/sh
systemctl status ggolbik-securitytools
sudo systemctl stop ggolbik-securitytools
sudo rm /etc/systemd/system/ggolbik-securitytools.service
sudo systemctl daemon-reload
echo "Removed ggolbik-securitytools.service"