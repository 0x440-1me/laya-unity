# Security Policy

## Supported versions

Security fixes currently target the latest `0.x` release and `main`.

## Reporting a vulnerability

Please use GitHub's private vulnerability reporting feature for this repository. Do not open a public issue for an unpatched vulnerability or include real credentials in a report.

Include the affected version, reproduction steps, impact, and any suggested mitigation. Maintainers will acknowledge a complete report as soon as practical.

## Credential guidance

A public Unity game binary cannot safely hide an API key. Use direct TypeSafe credentials only for local development. Released games should call a developer-controlled backend, which then calls TypeSafe.

The SDK never includes authorization headers or API keys in its exception messages. Consumers should apply the same rule to their own telemetry.
