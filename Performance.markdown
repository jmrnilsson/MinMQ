# Run 0
5 ms flush as ihosted service
1 single wrk thread 5 connections

Starting minmq_mmq-db_1              ... done                                                                                              Starting minmq_mmq-service-kestrel_1 ... done                                                                                              Starting minmq_mmq-service-express_1 ... done                                                                                              Starting minmq_mmq-service-nodejs_1  ... done                                                                                              
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
PS M:\devwork\MinMQ>
