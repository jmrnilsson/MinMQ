const winston = require('winston');

let _logger = null;

(function () {
    if (_logger != null) return;
    const { combine, timestamp, printf, splat, label } = winston.format;

    const customFormat = printf(({ level, label, message, timestamp }) => {
        return `${level}: [${label}] ${timestamp} ${message}`;
    });

    _logger = winston.createLogger({
        level: process.env.LOGLEVEL || 'info',
        format: combine(
            splat(),
            label({ label: 'Service.Nodejs' }),
            timestamp(),
            customFormat,
        ),
        defaultMeta: { service: 'user-service' },
        transports: [
            new winston.transports.Console(),
        ],
    });
}());

module.exports = _logger;
