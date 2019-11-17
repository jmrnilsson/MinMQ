# MinMQ

MinMQ is a minimal message queue for private networks for use primarily in Docker. It's designed for
low ceremony, high throughput, medium latency, HTTP-transport for transmission of small messages.

## Design
- C# 8
- Dotnet core 3
- IAsyncEnumerable
- Rx (maybe)
- RedHat Linux Socket Transport
- Load tested
- In-order processing
- Error queue-capable
- Named queues for concurrency control
- Limited to small messages (<256kB).

## Caveats
It's not designed for
- Transacted suites of messages. Use composite messages for this.

## Usage
With Docker-compose

    docker-compose build; \
    docker-compose down; \
    docker-compose run mmq-load-tests -- status.sh

Enter load-tests container

    docker-compose run load-tests --

Checkout out the nodejs expression server

    curl -X GET http://localhost:4000/status