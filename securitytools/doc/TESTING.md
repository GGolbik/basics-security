# Testing

This file contains the documentation of all tests of this project.

# Table of Contents

* [Best Bractices](#best-bractices)
* [Test Script](#test-script)
* [Test in VSCode](#test-in-vscode)
* [Test Result](#test-result)
* [Test Coverage](#test-coverage)
* [Test Cases](#test-cases)
  * [Auto](#auto)
  * [Manual](#manual)
    * [Installation](#installation)

# Best Bractices

[Unit testing best practices with .NET Core and .NET Standard](https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices)

# Test Script

The test for the project can be executed with the [test.sh](test.sh) script.
There is no script to execute the tests inside a docker container yet.

# Test in VSCode

The test can also be executed in VSCode with the [.NET Core Test Explorer](https://marketplace.visualstudio.com/items?itemName=formulahendry.dotnet-test-explorer) extension or by launching with `dotnet test` (see [SETUP](/SETUP.md#visual-studio-code)).

# Test Result

The test result can be found in `build/test/result.html`. 

![](images/testresult.png)

# Test Coverage

The coverage report can be found in `build/test/coverage/index.htm`.

![](images/coverage.png)

# Test Cases

## Auto

See source code in the [test](./test/) directory.

## Manual

### Installation

- Install on Windows self-contained
- Install on Windows framework-dependent
- Install on Windows as service
- Install on Linux self-contained
- Install on Linux framework-dependent
- Install on Linux as service
- Install on Linux with DEB
- Install on Linux with DEB as a service
- Install as PWA from HTTP with Chrome and Edge
- Install as PWA from HTTPS with Chrome and Edge
