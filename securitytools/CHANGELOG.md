# Changelog

All notable changes to this project will be documented in this file.

# [Unreleased]

- Update from .NET 7.0 to .NET 8.0
- Extend configuration of subject alternative name to support full specification.
- Add transform mode config which allows to transform an object into the related config.
- Add [Electron](https://www.electronjs.org/).
- Return human readable names in transform result.
- Add worker which can be used to run and cancel tasks.
- Add support to store key pairs in encrypted form.
- Add support for credentials.

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
