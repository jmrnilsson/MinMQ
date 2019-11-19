# MinMQ

MinMQ is a minimal message queue for private networks for use primarily in Docker. It's designed for
low ceremony, high throughput, medium latency, HTTP-transport for transmission of small messages.

## Design
- C# 8
- Dotnet core 3
- IAsyncEnumerable
- Rx (maybe). :skull: Hike!
- RedHat Linux Socket Transport. :skull: Dotnet core 3.0 unsupported.
- Load tested :heavy_check_mark:
- In-order processing :heavy_check_mark:
- Error queue-capable
- Named queues for concurrency control
- Limited to small messages (<256kB).

## Caveats
It's not designed for
- Transacted suites of messages. Use composite messages for this.

## Usage
With Docker-compose

    docker-compose build; docker-compose down; docker-compose run mmq-load-tests -- status.sh
    docker-compose build; docker-compose down; docker-compose run mmq-load-tests -- post_message.sh

Enter load-tests container

    docker-compose run mmq-load-tests --

Checkout out the nodejs expression server

    curl -X GET http://localhost:4000/status

Curl faster

    curl -H "Accept: application/json" -H "Content-type: application/json" -X POST -d '{"id":100}' http://localhost:9000/faster --trace-ascii /dev/stdout


## Some preliminary benchmarks
### EFCore In-Memory Database

Warning: The file name argument '-I' looks like a flag.
Warning: The file name argument '-I' looks like a flag.
Warning: The file name argument '-I' looks like a flag.
Running 5s test @ http://mmq-service-kestrel:9000/message-text
  2 threads and 3 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency     9.01ms   44.44ms 369.75ms   95.97%
    Req/Sec     2.79k   758.75     3.78k    86.46%
  26670 requests in 5.10s, 2.34MB read
Requests/sec:   5229.63
Transfer/sec:    469.85KB

### Faster

Running 5s test @ http://mmq-service-kestrel:9000/faster
  2 threads and 13 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency     1.12ms    2.69ms  45.34ms   97.36%
    Req/Sec     6.87k     2.12k    9.85k    75.00%
  32854 requests in 5.01s, 4.01MB read
Requests/sec:   6561.83
Transfer/sec:    820.23KB

### HTTP GET Compared
```
Running 10s test @ http://mmq-service-nodejs:8000/status
  12 threads and 400 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency    32.55ms    5.61ms 129.52ms   94.83%
    Req/Sec     1.02k   158.32     1.99k    83.97%
  120047 requests in 10.08s, 14.88MB read
Requests/sec:  11910.95
Transfer/sec:      1.48MB

Running 10s test @ http://mmq-service-express:4000/status
  12 threads and 400 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency    49.60ms    8.15ms 398.64ms   94.28%
    Req/Sec   655.91    132.34     1.00k    86.10%
  76784 requests in 10.07s, 16.40MB read
Requests/sec:   7621.72
Transfer/sec:      1.63MB

Running 10s test @ http://mmq-service-kestrel:9000/status
  12 threads and 400 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency    22.98ms    5.09ms  68.42ms   72.30%
    Req/Sec     1.43k   181.25     2.69k    74.83%
  171200 requests in 10.08s, 27.92MB read
Requests/sec:  16985.24
Transfer/sec:      2.77MB
PS M:\devwork\MinMQ>
``` 

### Kestrel HTTP POST - In-memory EFCore database - 1000 pre-loaded records
#### A test message

    Running 10s test @ http://mmq-service-kestrel:9000/message-text
      12 threads and 400 connections
      Thread Stats   Avg      Stdev     Max   +/- Stdev
        Latency    48.23ms   16.36ms 258.85ms   78.46%
        Req/Sec   672.28    170.82     1.03k    76.64%
      78927 requests in 10.07s, 6.92MB read
    Requests/sec:   7841.49
    Transfer/sec:    704.51KB

#### A very large message

    Running 10s test @ http://mmq-service-kestrel:9000/message-text
      12 threads and 400 connections
      Thread Stats   Avg      Stdev     Max   +/- Stdev
        Latency    52.20ms   18.95ms 290.34ms   82.02%
        Req/Sec   624.62    219.15     2.00k    91.12%
      70918 requests in 10.06s, 6.22MB read
      Socket errors: connect 0, read 22, write 0, timeout 0
    Requests/sec:   7052.28
    Transfer/sec:    633.60KB

#### JSON body

    Running 10s test @ http://mmq-service-kestrel:9000/message
      12 threads and 400 connections
      Thread Stats   Avg      Stdev     Max   +/- Stdev
        Latency    47.49ms    9.30ms  99.34ms   71.41%
        Req/Sec   682.04    121.71     1.29k    76.21%
      79843 requests in 10.02s, 7.01MB read
    Requests/sec:   7965.02
    Transfer/sec:    715.61KB
