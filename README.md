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
- A [free Hookdeck account](https://dashboard.hookdeck.com/signup?ref=github-aspire-webhooks) to be able to verify the webhooks

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

Get your webhook signing secret from the [Hookdeck dashboard](https://dashboard.hookdeck.com) -> **Settings** -> **Secrets** and store the value in a [secret storage](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=linux#enable-secret-storage).

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
2. Create a localtunnel using the Hookdeck CLI:
  ```sh
  hookdeck listen 5520 weather-webhook --path /webhooks/weather
  ```

  This creates a [Hookdeck Source](https://hookdeck.com/docs/sources?ref=github-aspire-webhooks) that forwards any requests to the CLI with the path `/webhooks/weather`.
3. Copy the `weather-webhook` Source URL from the CLI output:
  ```
  Dashboard
  👉 Inspect and replay events: https://dashboard.hookdeck.com?team_id=tm_{id}

  Sources
  🔌 weather-webhook URL: https://hkdk.events/{id}

  Connections
  weather-webhook -> weather-webhook_to_cli-weather-webhook forwarding to /webhooks/weather

  > Ready! (^C to quit)
  ```
4. Test the receipt of a webhook with a cURL request:
  ```sh
  curl --location 'https://hkdk.events/{id}' \
    --header 'Content-Type: application/json' \
    --data '[{"Date":"2024-09-18","TemperatureC":47,"Summary":"Cool","TemperatureF":116},{"Date":"2024-09-19","TemperatureC":37,"Summary":"Chilly","TemperatureF":98},{"Date":"2024-09-20","TemperatureC":13,"Summary":"Sweltering","TemperatureF":55},{"Date":"2024-09-21","TemperatureC":40,"Summary":"Chilly","TemperatureF":103},{"Date":"2024-09-22","TemperatureC":-7,"Summary":"Cool","TemperatureF":20}]'
  ```
5. Check the **webhooksservice** Console logs within the .NET Aspire dashboard. You'll see something similar to the following:
  ```
  2024-10-09T13:58:41.9360000 Webhook originated from Hookdeck
  2024-10-09T13:58:41.9610000 Received JSON: WeatherForecast { Date = 9/18/2024, TemperatureC = 47, Summary = Cool, TemperatureF = 116 }
  2024-10-09T13:58:41.9760000 Stored forecast in cache
  ```

See [next steps](next-steps.md) for the general instructions for deploying to Azure.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
