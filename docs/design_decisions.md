## Libuv 
For the time beeing this version reverts back from Managed sockets to widely adopted Libuv-transport previously which
was used in AspNetCore 1.0 and replaced and later on restored [restored](https://github.com/aspnet/KestrelHttpServer/issues/2104)
in AspNet 2.1. Similarl to [this article](https://github.com/aspnet/KestrelHttpServer/issues/2104) it has been found 
that it performs significantly better in high-contention scenarios but slightly worse under balanced load.
