# LettuceIO

A groundbreaking tool developed in `.NET` for effortless
[RabbitMQ](https://rabbitmq-website.pages.dev) data publishing and consumption.
This innovative application revolutionizes the RabbitMQ experience,
offering users a seamless interface powered by
[Chromium](https://www.chromium.org/chromium-projects).
With `LettuceIO` you can easily manage exchanges and queues,
publish messages and consume data - all without writing a single line of code.
Simply navigate the intuitive Chromium interface to streamline your data flow,
making RabbitMQ operations as easy as point-and-click.

![Lettuce.IO Example](/.docs/images/lettuce-io-view.PNG)

## Recording Data

You can record data from both exchanges and queues to simulate specific data
flows or to test your microservices with real data from
production environments.

The data will be stored in a directory of your choice in the following
json format:

```json
{
  "RoutingKey": "/",
  "Body": "TXkgRGF0YSBIZXJlIQ==",
  "TimeDelta": "00:00:11.6557305"
}
```

![Recoding data using Lettuce.IO](/.docs/images/lettuce-io-recording.PNG)

## Publishing Data

To publish messages, choose a folder and a destination exchange or queue
and press start!

![Publishing data using Lettuce.IO](/.docs/images/lettuce-io-publishing.PNG)

In this example we publish the same message at `1,500` messages per second:

![RabbitMQ Exchange Example](/.docs/images/rabbitmq-publishing-to-an-exchange.PNG)
