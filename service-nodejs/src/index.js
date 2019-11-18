const http = require('http');
const logger = require('./logger.js');
const port = 8000

const requestHandler = (req, res) => {
  const url = req.url || "";
  if (url.match(/^\/status\/?$/)) {
    // logger.info('Url=%s', req.url);
    res.writeHead(200);
    return res.end('{text: "ok"}');
  }
  res.writeHead(404);
  return res.end('{error: 404}');

}

const server = http.createServer(requestHandler)


server.listen(port, (err) => {
  if (err) {
    return logger.error('Something bad happened', err)
  }

  logger.info('Listening. Port=%s', port);
});

