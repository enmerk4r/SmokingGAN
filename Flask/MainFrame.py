import os
from flask import Flask, jsonify, request, Blueprint
import json

from Blueprints.style_gan import StyleGAN
from Blueprints.depth_map import DepthMap

app = Flask(__name__)
app.register_blueprint(StyleGAN)
app.register_blueprint(DepthMap)

if __name__ == '__main__':
    app.run(host='127.0.0.1')