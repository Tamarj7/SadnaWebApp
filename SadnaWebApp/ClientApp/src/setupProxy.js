const { createProxyMiddleware } = require('http-proxy-middleware');
const { env } = require('process');

module.exports = function (app) {
    // Proxy requests for the users database controller
    app.use(
        '/usersDB',
        createProxyMiddleware({
            target: 'http://localhost:5023',
            changeOrigin: true,
        })
    );

    // Proxy requests for the top scores database controller
    app.use(
        '/topScoresDB',
        createProxyMiddleware({
            target: 'http://localhost:5023',
            changeOrigin: true,
        })
    );
};
