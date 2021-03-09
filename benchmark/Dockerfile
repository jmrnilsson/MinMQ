FROM ubuntu:focal-20210217 AS build

ENV DEBIAN_FRONTEND noninteractive

RUN apt-get update

RUN apt-get install -y --no-install-recommends \
    ca-certificates \
    git \
    build-essential \
    wget \
    unzip

WORKDIR /source
# RUN git clone https://github.com/wg/wrk.git app/wrk
RUN wget https://github.com/wg/wrk/archive/master.zip
# RUN wget https://github.com/jmrnilsson/wrk/archive/master.zip
RUN unzip master.zip
COPY ARM64v8-atomic-op-fix.diff /source/wrk-master/
RUN (cd wrk-master; patch -p1 < ./ARM64v8-atomic-op-fix.diff; make)

FROM ubuntu:focal-20210217 AS mmq-benchmark

RUN apt-get update

RUN apt-get install -y --no-install-recommends \
    ca-certificates \
    curl

COPY --from=build /source/wrk-master/ /app/wrk/
COPY install.sh /opt/install.sh
COPY *.lua /app/wrk/scripts/
RUN chmod +x /opt/install.sh

WORKDIR /app/wrk

RUN rm -rf /var/lib/apt/lists/*
RUN rm -rf /src/*.deb
RUN apt-get clean

RUN sh /opt/install.sh 2>&1 | tee -a "install_sh.log"

ENTRYPOINT [ "bash" ]
