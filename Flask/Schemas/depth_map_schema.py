from marshmallow import Schema, fields, ValidationError, pre_load, post_load, pre_dump, post_dump
from PIL import Image
from io import BytesIO
import base64

class DepthMapInput(object):
    def __init__(self, inputImage):
        self.inputImage = inputImage

class DepthMapInputSchema(Schema):
    inputImage = fields.String(required=True)

    @post_load
    def make_DepthMapInputSchema(self, data, **kwargs):
        inp = Image.open(BytesIO(base64.b64decode(data['inputImage'])))
        return DepthMapInput(inp)