# OtpGenerator

[![Build Status](https://github.com/davewalker5/OtpGenerator/workflows/.NET%20Core%20CI%20Build/badge.svg)](https://github.com/davewalker5/OtpGenerator/actions)
[![GitHub issues](https://img.shields.io/github/issues/davewalker5/OtpGenerator)](https://github.com/davewalker5/OtpGenerator/issues)
[![Coverage Status](https://coveralls.io/repos/github/davewalker5/OtpGenerator/badge.svg?branch=main)](https://coveralls.io/github/davewalker5/OtpGenerator?branch=master)
[![Releases](https://img.shields.io/github/v/release/davewalker5/OtpGenerator.svg?include_prereleases)](https://github.com/davewalker5/OtpGenerator/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/davewalker5/OtpGenerator/blob/master/LICENSE)
[![Language](https://img.shields.io/badge/language-c%23-blue.svg)](https://github.com/davewalker5/OtpGenerator/)
[![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/davewalker5/OtpGenerator)](https://github.com/davewalker5/OtpGenerator/)

## About

The OTP generator implements a desktop (TOTP) timed one-time password or 2FA code generator featuring:

- Secure, encrypted, file-based storage of service definitions
- Addition and removal of services from the data file and listing of the current content
- Data exchange via CSV, to facilitate importing and exporting services to and from the application via CSV file
- One-off TOTP generation
- "Live View" continuous TOTP generation with codes refreshing as they expire

## Getting Started

Please see the [Wiki](https://github.com/davewalker5/OtpGenerator/wiki) for configuration details and the user guide.

## Authors

- **Dave Walker** - _Initial work_ - [GitHub](https://github.com/davewalker5)

## Credits

The application uses the Otp.NET NuGet package by Kyle Spearrin for TOTP code generation:

- [Otp.NET](https://github.com/kspearrin/Otp.NET)

It uses the "Konscious.Security.Cryptography.Argon2" NuGet package by Keef Aragon for encryption key generation:

- [Konscious.Security.Cryptography.Argon2](https://github.com/kmaragon/Konscious.Security.Cryptography)

## Feedback

To file issues or suggestions, please use the [Issues](https://github.com/davewalker5/OtpGenerator/issues) page for this project on GitHub.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
