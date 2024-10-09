# Webhooks with .NET Aspire

This project demonstrates how to implement and handle webhooks using [.NET Aspire](https://github.com/dotnet/aspire).

## Table of Contents

- [Introduction](#introduction)
- [Features](#features)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Installation](#installation)
- [Usage](#usage)
- [Contributing](#contributing)
- [License](#license)

## Introduction

[Webhooks](https://hookdeck.com/webhooks/guides/what-are-webhooks-how-they-work?ref=github-aspire-webhooks) are a way for web services to communicate with each other by sending data to a specified URL. This is most commonly an HTTP POST request.

This project provides a sample receiving webhooks using .NET Aspire. Using [Hookdeck](https://hookdeck.com?ref=github-aspire-webhooks) with a .NET Aspire application gives you full observability of both internal services via Aspire and communication with external service via Hookdeck.

## Features

- Receive and process webhook events
- Validate webhook payloads
- Log webhook events

## Project Structure

- **Web**: Web frontend
- **ApiService**: Internal API
- **WebhooksService**: Receive external webhooks
- **ServiceDefault**
- **AppHost**

## Getting Started

To get started with this project, follow the instructions below.

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [The Hookdeck CLI](https://hookdeck.com/docs/cli?ref=github-aspire-webhooks) for receiving webhooks on the localhost

### Installation

1. Clone the repository:
  ```sh
  git clone https://github.com/leggetter/AspireWebhook.git
  ```
2. Navigate to the project directory:
  ```sh
  cd AspireWebhook
  ```

## Configuration

```sh
dotnet user-secrets init --project "./AspireWebhooks/AspireWebhooks.WebhooksService"
dotnet user-secrets set "AspireWebhooks:HookdeckWebhookSecret" "YOUR-WEBHOOK-SECRET" \
  --project "./AspireWebhooks/AspireWebhooks.WebhooksService"
```

## Usage

1. Build and run the project:
  ```sh
  ./start.sh
  ```

🚧 More information coming soon...

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
