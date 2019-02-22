import json
import logging

from flask import Flask, g
from flask_oidc import OpenIDConnect

logging.basicConfig(level=logging.DEBUG)

app = Flask(__name__)
app.config.update({
    'SECRET_KEY': 'Secret Key For Flask App',
    
    'OIDC_CLIENT_SECRETS': 'client_secrets.json',
    'OIDC_ID_TOKEN_COOKIE_SECURE': False,
    'OIDC_SCOPES': ['openid', 'email', 'profile', 'api1']
})
oidc = OpenIDConnect(app)


@app.route('/')
def hello_world():
    if oidc.user_loggedin:
        return ('Hello, %s, <a href="/private">See private</a> '
                '<a href="/logout">Log out</a>') % \
            oidc.user_getfield('email')
    else:
        return 'Welcome anonymous, <a href="/private">Log in</a>'


@app.route('/private')
@oidc.require_login
def hello_me():
    info = oidc.user_getinfo(['email', 'openid_id', 'name', 'preferred_username'])
    return ('Hello, %s (%s)! Visit <a href="http://localhost:7001">MVC Client</a> and <a href="http://localhost:7000">Hybrid Client</a> and <a href="http://localhost:4200">Angular Client</a>, <a href="/">Return</a>' %
            (info.get('name'), info.get('preferred_username')))


@app.route('/api')
@oidc.accept_token(True, ['openid'])
def hello_api():
    return json.dumps({'hello': 'Welcome %s' % g.oidc_token_info['sub']})


@app.route('/logout')
def logout():
    oidc.logout()
    return 'Hi, you have been logged out! <a href="/">Return</a>'
    
if __name__ == '__main__':
    app.run('localhost', debug=True, port=7002)