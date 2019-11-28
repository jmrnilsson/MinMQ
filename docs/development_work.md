# Development work details
## Setup database 
Start the databaste with 

    docker-compose up mmq-db

Start Visual Studio since the dotnet core tools from 2.* doesn't work any more or [documenation](https://docs.microsoft.com/en-us/ef/core/get-started/?tabs=netcore-cli) is not up-to-date.

Tools > Nuget Package Mannager

    Update-Database -Context MessageQueueContext

## Benchmarking
The benchmarking solution currently makes comparisons agains nodejs's http module and nodejs express. Moreover, FASTER
is compared agains in-memory databases.

### How to benchmark
With Docker-compose. `Sudo` is system dependant doesn't have to apply.

    docker-compose build; docker-compose down; docker-compose up;
    sudo docker-compose run mmq-benchmarks -- status.sh
  
Or, 
    sudo docker-compose run mmq-benchmarks -- post_message.sh

Or just enter benchmark container,

    docker-compose run mmq-benchmark --

Checkout out the Nodejs expression server

    curl -X GET http://localhost:4000/status

Check the log

    curl -X GET http://localhost:9000/list


Troubleshoot with curl

    curl -H "Accept: application/json" -H "Content-type: application/json" -X POST -d '{"id":"asdad"}' http://localhost:9000/faster --trace-ascii /dev/stdout

## Links 
- https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/write?view=aspnetcore-3.0#per-request-middleware-dependencies
- https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.0