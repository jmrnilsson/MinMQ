FROM node:11-slim

ENV DEBIAN_FRONTEND noninteractive

RUN apt-get update
RUN apt-get install -y --no-install-recommends ca-certificates
RUN rm -rf /var/lib/apt/lists/*
RUN apt-get purge --auto-remove -y curl
RUN rm -rf /src/*.deb
RUN apt-get clean

COPY . /app/
WORKDIR /app

RUN npm install
RUN npm run build
RUN rm -dr src

EXPOSE 1000

CMD ["node", "./dist/index.js"]
