FROM ubuntu:disco AS build

ENV DEBIAN_FRONTEND noninteractive

RUN apt-get update

RUN apt-get install -y --no-install-recommends \
    ca-certificates \
    git \
    build-essential \
    wget \
    unzip \
    gifsicle \
    ffmpeg

COPY minmq.mov ./

# docker build . -t gifd
# docker run -it -t gifd
# 0: ffmpeg -i minmq-desktop.mov -s 960x540   -pix_fmt rgb8 -r 10 -f gif - | gifsicle --optimize=3 --delay=3 > out.gif
# 1: ffmpeg -i minmq.mov -pix_fmt rgb8 -r 3 -f gif - | gifsicle --optimize=3 --delay=100 > out.gif 
# new terminal:
# docker ps -a
# docker cp 702128e33d87:/out.gif ./
ENTRYPOINT [ "bash" ]