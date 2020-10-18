from marshmallow import Schema, fields, ValidationError, pre_load, post_load, pre_dump, post_dump
from PIL import Image
from io import BytesIO
import base64

class StyleTransferInput(object):
    def __init__(self, inputImage, style):
        self.inputImage = inputImage
        self.style = style

class StyleTransferInputSchema(Schema):
    inputImage = fields.String(required=True)
    style = fields.String(required=True)

    @post_load
    def make_StyleTransferInput(self, data, **kwargs):
        inp = Image.open(BytesIO(base64.b64decode(data['inputImage'])))
        return StyleTransferInput(inp, data['style'])

class StyleTransferOutput(object):
    def __init__(self, result):
        self.result = result

class StyleTransferOutputSchema(Schema):
    result = fields.String(required=True)

    @post_load
    def make_StyleTransferOutput(self, data, **kwargs):
        return StyleTransferOutput(data['result'])