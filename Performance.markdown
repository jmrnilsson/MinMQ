# Run 0
- 5 ms commit flush in aspnet hosted service
- 1 single wrk thread 5 connections

```
- Make sure to check CPU and RAM saturation.

Warning: The file name argument '-I' looks like a flag.
Warning: The file name argument '-I' looks like a flag.
Warning: The file name argument '-I' looks like a flag.
Running 12s test @ http://mmq-service-kestrel:9000/message-text
  1 threads and 5 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency     6.34ms   35.46ms 377.14ms   97.50%
    Req/Sec     7.08k     1.36k    8.59k    84.75%
  83185 requests in 12.10s, 7.30MB read
Requests/sec:   6875.01
Transfer/sec:    617.68KB
Running 12s test @ http://mmq-service-kestrel:9000/faster
  1 threads and 5 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency   404.43us  711.85us  25.12ms   98.65%
    Req/Sec    13.88k     1.59k   15.66k    85.95%
  167135 requests in 12.10s, 20.40MB read
Requests/sec:  13812.66
Transfer/sec:      1.69MB
```

# Run 1
1 single wrk thread 5 connections
semaphore 12
CommitAsync inlined and scheduled 5 in hosted service

```
Running 5s test @ http://mmq-service-kestrel:9000/message-text
  1 threads and 5 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency    14.18ms   54.69ms 384.68ms   94.52%
    Req/Sec     6.24k     1.79k    7.99k    80.85%
  29224 requests in 5.00s, 2.56MB read
Requests/sec:   5844.06
Transfer/sec:    525.05KB
Running 7s test @ http://mmq-service-kestrel:9000/faster
  1 threads and 5 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency   603.35us    2.03ms  44.94ms   98.25%
    Req/Sec    12.60k     1.92k   15.40k    72.86%
  87728 requests in 7.00s, 10.71MB read
Requests/sec:  12529.97
Transfer/sec:      1.53MB
Running 7s test @ http://mmq-service-kestrel:9000/faster
  2 threads and 15 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency   730.05us  405.16us  19.18ms   91.86%
    Req/Sec     8.62k     2.31k   10.73k    85.71%
  12010 requests in 7.01s, 1.47MB read
  Socket errors: connect 0, read 14, write 5728, timeout 0
  Non-2xx or 3xx responses: 1
Requests/sec:   1713.00
Transfer/sec:    214.12KB
./wrk: invalid option -- '1'
```


# Run 2 
Same as above except semaphore 8

```
  Running 5s test @ http://mmq-service-kestrel:9000/message-text
    1 threads and 5 connections
    Thread Stats   Avg      Stdev     Max   +/- Stdev
      Latency    13.87ms   55.17ms 397.95ms   94.66%
      Req/Sec     6.21k     1.86k    8.49k    65.96%
    29041 requests in 5.00s, 2.55MB read
  Requests/sec:   5807.17
  Transfer/sec:    521.74KB
  Running 7s test @ http://mmq-service-kestrel:9000/faster
    1 threads and 5 connections
    Thread Stats   Avg      Stdev     Max   +/- Stdev
      Latency   435.85us    0.91ms  25.67ms   98.68%
      Req/Sec    13.43k     1.55k   15.29k    83.10%
    94885 requests in 7.10s, 11.58MB read
  Requests/sec:  13364.14
  Transfer/sec:      1.63MB
  Running 7s test @ http://mmq-service-kestrel:9000/faster
    2 threads and 15 connections
    Thread Stats   Avg      Stdev     Max   +/- Stdev
      Latency     0.95ms    1.14ms  19.11ms   94.96%
      Req/Sec     8.91k     1.22k   11.60k    72.14%
    124084 requests in 7.00s, 15.15MB read
  Requests/sec:  17723.17
  Transfer/sec:      2.16MB
  ./wrk: invalid option -- '1'
```

## Error count 1. See below:

```
mmq-service-kestrel_1  | fail: Microsoft.AspNetCore.Server.Kestrel[13]
mmq-service-kestrel_1  |       Connection id "0HLRCRPGBP8G2", Request id "0HLRCRPGBP8G2:000021DD": An unhandled exception was thrown by the application.
mmq-service-kestrel_1  | Microsoft.AspNetCore.Server.Kestrel.Core.BadHttpRequestException: Unexpected end of request content.
mmq-service-kestrel_1  |    at Microsoft.AspNetCore.Server.Kestrel.Core.BadHttpRequestException.Throw(RequestRejectionReason reason)
mmq-service-kestrel_1  |    at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.Http1ContentLengthMessageBody.ReadAsyncInternal(CancellationToken cancellationToken)
mmq-service-kestrel_1  |    at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpRequestStream.ReadAsyncInternal(Memory`1 buffer, CancellationToken cancellationToken)
mmq-service-kestrel_1  |    at System.IO.StreamReader.ReadBufferAsync()
mmq-service-kestrel_1  |    at System.IO.StreamReader.ReadToEndAsyncInternal()
mmq-service-kestrel_1  |    at Service_Kestrel.Startup.<>c.<<HandleFasterRun>b__11_0>d.MoveNext() in /src/Startup.cs:line 101
mmq-service-kestrel_1  | --- End of stack trace from previous location where exception was thrown ---
mmq-service-kestrel_1  |    at Microsoft.AspNetCore.Builder.Extensions.MapMiddleware.Invoke(HttpContext context)
mmq-service-kestrel_1  |    at Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http.HttpProtocol.ProcessRequests[TContext](IHttpApplication`1 application)
mmq-service-kestrel_1  | info: Service_Kestrel.Faster.FasterCommitHostedService[0]
```

# Run 3
Same as above (see run 2)

```
- Make sure to check CPU and RAM saturation.

Warning: The file name argument '-I' looks like a flag.
Warning: The file name argument '-I' looks like a flag.
Warning: The file name argument '-I' looks like a flag.
Running 5s test @ http://mmq-service-kestrel:9000/message-text
  1 threads and 5 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency    14.03ms   54.66ms 395.86ms   94.74%
    Req/Sec     6.17k     1.78k    8.38k    68.09%
  28896 requests in 5.00s, 2.54MB read
Requests/sec:   5777.66
Transfer/sec:    519.09KB
Running 7s test @ http://mmq-service-kestrel:9000/faster
  1 threads and 5 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency   459.33us    0.93ms  29.67ms   98.74%
    Req/Sec    12.69k     1.87k   14.91k    82.86%
  88370 requests in 7.00s, 10.79MB read
Requests/sec:  12623.80
Transfer/sec:      1.54MB
Running 7s test @ http://mmq-service-kestrel:9000/faster
  2 threads and 15 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency     1.07ms    1.36ms  20.20ms   94.01%
    Req/Sec     8.27k     1.32k   13.68k    72.34%
  116066 requests in 7.10s, 14.17MB read
Requests/sec:  16347.69
Transfer/sec:      2.00MB
Running 7s test @ http://mmq-service-kestrel:9000/faster
  12 threads and 400 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency    21.62ms    8.38ms 141.03ms   87.89%
    Req/Sec     1.52k   346.36     2.30k    73.81%
  127299 requests in 7.07s, 15.54MB read
Requests/sec:  18012.16
Transfer/sec:      2.20MB
PS M:\devwork\MinMQ>
```

# Run 4
Same as above (see run 2)

```
  > docker-compose run mmq-load-tests -- post_message.sh
                                                    
  - Make sure to check CPU and RAM saturation.

  Warning: The file name argument '-I' looks like a flag.
  Warning: The file name argument '-I' looks like a flag.
  Warning: The file name argument '-I' looks like a flag.
  Running 5s test @ http://mmq-service-kestrel:9000/message-text
    1 threads and 5 connections
    Thread Stats   Avg      Stdev     Max   +/- Stdev
      Latency    13.86ms   53.68ms 394.64ms   94.47%
      Req/Sec     6.08k     1.66k    8.43k    65.96%
    28456 requests in 5.00s, 2.50MB read
  Requests/sec:   5690.37
  Transfer/sec:    511.24KB
  Running 7s test @ http://mmq-service-kestrel:9000/faster
    1 threads and 5 connections
    Thread Stats   Avg      Stdev     Max   +/- Stdev
      Latency   786.12us    4.41ms  81.17ms   98.93%
      Req/Sec    12.75k     2.02k   15.04k    81.43%
    88752 requests in 7.00s, 10.83MB read
  Requests/sec:  12676.25
  Transfer/sec:      1.55MB
  Running 7s test @ http://mmq-service-kestrel:9000/faster
    2 threads and 15 connections
    Thread Stats   Avg      Stdev     Max   +/- Stdev
      Latency     0.93ms    0.96ms  11.94ms   93.96%
      Req/Sec     8.70k     1.20k   14.85k    69.50%
    121995 requests in 7.10s, 14.89MB read
  Requests/sec:  17183.07
  Transfer/sec:      2.10MB
  Running 7s test @ http://mmq-service-kestrel:9000/faster
    12 threads and 400 connections
    Thread Stats   Avg      Stdev     Max   +/- Stdev
      Latency    20.31ms    7.36ms  73.75ms   83.62%
      Req/Sec     1.62k   351.02     3.70k    80.72%
    134040 requests in 7.07s, 16.36MB read
  Requests/sec:  18957.32
  Transfer/sec:      2.31MB
```

# Run 5

Running 5s test @ http://mmq-service-kestrel:9000/message-text
  1 threads and 5 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency    13.53ms   52.21ms 377.62ms   94.74%
    Req/Sec     6.53k     1.85k    8.51k    74.47%
  30570 requests in 5.00s, 2.68MB read
Requests/sec:   6113.29
Transfer/sec:    549.24KB
Running 7s test @ http://mmq-service-kestrel:9000/faster
  1 threads and 5 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency   468.50us    1.00ms  28.30ms   98.83%
    Req/Sec    12.52k     2.21k   14.96k    77.14%
  87122 requests in 7.00s, 10.22MB read
Requests/sec:  12445.50
Transfer/sec:      1.46MB
Running 7s test @ http://mmq-service-kestrel:9000/faster
  2 threads and 15 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency     0.97ms    1.09ms  19.24ms   93.91%
    Req/Sec     8.66k     1.47k   13.78k    72.34%
  121423 requests in 7.10s, 14.24MB read
Requests/sec:  17101.68
Transfer/sec:      2.01MB
Running 7s test @ http://mmq-service-kestrel:9000/faster
  12 threads and 400 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency    19.10ms    7.08ms 115.85ms   85.94%
    Req/Sec     1.73k   344.00     3.64k    79.88%
  144830 requests in 7.08s, 16.99MB read
Requests/sec:  20441.80
Transfer/sec:      2.40MB

# Run 6

Running 5s test @ http://mmq-service-kestrel:9000/efcore-in-mem-text
  1 threads and 5 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency   295.74us  442.69us  10.03ms   98.11%
    Req/Sec    18.43k     2.63k   21.84k    86.27%
  93459 requests in 5.10s, 8.82MB read
  Non-2xx or 3xx responses: 93459
Requests/sec:  18325.97
Transfer/sec:      1.73MB
Running 7s test @ http://mmq-service-kestrel:9000/faster
  1 threads and 5 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency   479.06us    1.21ms  29.98ms   98.81%
    Req/Sec    13.02k     1.92k   15.10k    81.43%
  90734 requests in 7.00s, 10.64MB read
Requests/sec:  12958.56
Transfer/sec:      1.52MB
Running 7s test @ http://mmq-service-kestrel:9000/faster
  2 threads and 15 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency     1.05ms    1.29ms  19.99ms   93.75%
    Req/Sec     8.40k     1.22k   10.95k    72.86%
  117051 requests in 7.00s, 13.73MB read
Requests/sec:  16718.16
Transfer/sec:      1.96MB
Running 7s test @ http://mmq-service-kestrel:9000/faster
  12 threads and 400 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency    20.15ms   10.53ms 230.57ms   86.33%
    Req/Sec     1.66k   408.57     3.31k    80.40%
  131969 requests in 7.05s, 15.48MB read
Requests/sec:  18711.49
Transfer/sec:      2.19MB
