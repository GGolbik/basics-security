# Changelog

All notable changes to this project will be documented in this file.

# [Unreleased]

# v1.0.3

- Fix different build number for deb package.
    - Now the deb package has the same build number.
- Fix ignored request for file encoding.
    - Now the requested file encoding (PEM, DER) is respected.

# v1.0.2

- Fix generation of certificates and CSRs with the ExtendedKeyUsage extension.
    - The numeric value of the enum flags was wrong.

# v1.0.1

- Remove unnecessary dependency from debian package.
- Update name of debian package.

# v1.0.0

- Initial release.
