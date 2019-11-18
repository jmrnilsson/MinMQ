FROM ubuntu:eoan-20191017 AS build

ENV DEBIAN_FRONTEND noninteractive

RUN apt-get update

RUN apt-get install -y --no-install-recommends \
    ca-certificates \
    git \
    build-essential \
    wget \
    unzip

# RUN git clone https://github.com/wg/wrk.git app/wrk
# WORKDIR /app/wrk
# RUN make

WORKDIR /app/wrk
RUN wget https://github.com/wg/wrk/archive/master.zip
RUN unzip master.zip
RUN (cd wrk-master; make)

FROM ubuntu:eoan-20191017 AS mmq-load-tests

RUN apt-get update

RUN apt-get install -y --no-install-recommends \
    ca-certificates \
    curl

# COPY --from=build /app/wrk/ /app/wrk/
COPY --from=build /app/wrk/wrk-master/ /app/wrk/
COPY install.sh /opt/install.sh
COPY post.lua /app/wrk/post.lua
COPY post-json.lua /app/wrk/post-json.lua
COPY post-large.lua /app/wrk/post-large.lua
RUN chmod +x /opt/install.sh

WORKDIR /app/wrk

RUN rm -rf /var/lib/apt/lists/*
RUN rm -rf /src/*.deb
RUN apt-get clean

RUN sh /opt/install.sh 2>&1 | tee -a "install_sh.log"

ENTRYPOINT [ "bash" ]