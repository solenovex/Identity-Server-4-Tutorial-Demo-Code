'use strict';

const express = require('express'),
  cors = require('cors'),
  helmet = require('helmet'),
  morgan = require('morgan'),
  debug = require('debug')('app'),
  errorHandler = require('errorhandler'),
  jwt = require('express-jwt'),
  jwksClient = require('jwks-rsa');

const app = express();
const isDevelopment = process.env.NODE_ENV === 'development';
const PORT = process.env.PORT || 5002;

app.use(helmet());
app.use(cors());
app.use(morgan('dev'));

if (isDevelopment) app.use(errorHandler());

const issuer = 'http://localhost:5000';

const auth = jwt({
  secret: jwksClient.expressJwtSecret({
    cache: true,
    rateLimit: true,
    jwksRequestsPerMinute: 2,
    jwksUri: `${issuer}/.well-known/openid-configuration/jwks`
  }),

  audience: 'api2',
  issuer: issuer,
  algorithms: ['RS256']
});

app.get('/me', auth, (req, res) => {
  const user = req.user;
  debug('req.user: %O', user);

  return res.json(user);
});

app.get('/', (req, res) => {
  return res.send('Hello');
});

app.use((err, req, res, next) => {
  debug('%O', err);
  res.status(err.status || 500);

  if (isDevelopment) return res.json(err);
});

app.listen(PORT, () => {
  console.log(`API listening at http://localhost:${PORT}`);
});
