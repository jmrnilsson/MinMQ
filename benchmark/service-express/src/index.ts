import express from 'express';
import bodyParser from 'body-parser';
import { logger } from './logger';

const config = {
  name: 'mmq-service-nodejs (express)',
  port: 4000,
  host: '0.0.0.0',
};

const app = express();

app.use(bodyParser.json());

app.get('/status', (req, res) => {
  return res.status(200).json({
    text: 'ok'
  });
});

// server maybe used closing down later
const server = app.listen(config.port, config.host, async (e) => {
  if (e) {
    throw new Error('Internal Server Error');
  }

  const { name, host, port} = config;
  logger.info('Starting service. Name=%s Host=%s Port=%s', name, host, port);
});

// Take it cool and shutdown with ease.
// https://gist.github.com/jmrnilsson/2b2775e18207e52cf19d2544382f0943
// https://stackoverflow.com/questions/18771707/how-to-flush-winston-logs
// https://github.com/winstonjs/winston/issues/228
// https://medium.com/@becintec/building-graceful-node-applications-in-docker-4d2cd4d5d392
// --> https://github.com/winstonjs/winston/issues/1250
function shutdown(reason: string) {
  return () => {
    logger.on('finish', () => { process.exit(3); });
    logger.info('Process exiting. Reason=%s', reason);
    logger.end();
    setTimeout(() => process.exit(2), 2000);
  };
}

process.on('SIGHUP', shutdown('SIGHUP'));
process.on('SIGINT', shutdown('SIGINT'));
process.on('SIGTERM', shutdown('SIGTERM'));
process.on('uncaughtException', e => shutdown(String(e)));