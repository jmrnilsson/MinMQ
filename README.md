# MinMQ

**Note:** _This is a work-in-progress. So far only prototype API is in place for write queries. Nothing exists in terms a
managed or defined API for sending or retrieving messages._ 

MinMQ is a minimal message queue for private networks, on-premise, or atleast non-public networks. It targets virtual
machines, Docker and physical hosts. It's designed for low ceremony, high throughput, medium-to-low latency, and a 
HTTP-transport for comfortable transmission of small messages.

This effort focuses on having:
- Below 50 ms latency
- Throughput of at least 10 000 messages per second
- Transactional commits that are durable
- Continuous benchmarks mend discoverability  of performance hits
- In-order processing

The implementation merely combines the efforts of [microsoft.FASTER](https://github.com/microsoft/FASTER) and 
[AspNet Core 3.0](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-3.0); A HTTP-transports on top of a very
fast file handler. FASTER provides "group commits" with [Concurrent Prefix Recovery](https://www.microsoft.com/en-us/research/uploads/prod/2019/01/cpr-sigmod19.pdf) (CDR) over a [Work Ahead Log](https://wiki.postgresql.org/wiki/Improve_the_performance_of_ALTER_TABLE_SET_LOGGED_UNLOGGED_statement)
(WAL). This approach to transactions is reminiscent to that of [Microsoft Message Queue](https://support.microsoft.com/ms-my/help/256096/how-to-install-msmq-2-0-to-enable-queued-components) (MSMQ) for messages transacted in bulk when using the Microsoft
Distributed Transaction Coordinator (MSDTC). But it's quite diffent from "unlogged" tables or in-memory database 
flushing to a durable disk later on. 

Further more, this version reverts back from Managed sockets to the
Libuv-transport previously was used in AspNetCore 1.0 and [restored](https://github.com/aspnet/KestrelHttpServer/issues/2104) in AspNet 2.*. Similar to [this article](https://github.com/aspnet/KestrelHttpServer/issues/2104) it's found that performance drops
significantly in high-contention scenarios.

Future aspirations may include:
- A formal API (perhaps somthing reminiscent to MSMQ or IronMQ)
  - `Send` (or Post)
  - Explicit two-phase commits for retrieving messages:
    - `Peek` (or Reserve)
    - `Delete` (or Recieve or Get)  
- Swagger/Swashbuckle
- More [advanced benchmarks suites](https://github.com/aspnet/Benchmarks).
- A Docker image and/or [Helm charts](https://helm.sh/).
- Error-queues.
- Named queues for concurrency control.
- Limitation to small messages (<256kB).
- Possibly a client, or and example implementation of set of atomic yet composite messages.
- Materialized views, cached responses or read models.


## Setup
FASTER allocates disk preemptively. Around 1.1 GB is used per default. Consequently a large docker volume, or path on
disk that comfortably can allocate more than 1.1GB have to be assigned, preferably an SSD.

**For docker users:**
Inspect the `setup.ps1` and change path so corresponds some disk space. For comfort i may be simpler change it to
shell-script instead.

**For all others**
If you plan to run the service without a container service the path set as FasterDevice in [appsettings.Development.json](./service-kestrel/Service-Kestrel/Service-Kestrel/appsettings.Development.json) must be assigned.

## Usage
With Docker-compose

    docker-compose build; docker-compose up;
    docker-compose run mmq-benchmark -- status.sh
  
Or, 
    docker-compose run mmq-benchmark -- post_message.sh

Or just enter benchmark container,

    docker-compose run mmq-benchmark --

Checkout out the Nodejs expression server

    curl -X GET http://localhost:4000/status

Troubleshoot with curl

    curl -H "Accept: application/json" -H "Content-type: application/json" -X POST -d '{"id":100}' http://localhost:9000/faster --trace-ascii /dev/stdout

## Some preliminary benchmarks

Some ad-hoc performance comparisons have been made [comparing both different web servers]
(./web-performance.md); Mostly those kinds that rely on Libuv. Moreover, benchmarks are mainly focused towards
seeing commit-over-commit performance hits. Metrics in focus are really just _throughput_ and _latency_. Some
unstructured logs of benchmarks runs can be found in [performance.md](./performance.md).