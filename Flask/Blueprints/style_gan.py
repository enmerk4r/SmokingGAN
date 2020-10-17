from flask import Flask, jsonify, request, Blueprint
import os
import Schemas.style_gan_schema as StyleGanSchemata
from pprint import pprint
from PIL import Image, ImageFilter
from io import BytesIO, StringIO
import base64

styleGanInputSchema = StyleGanSchemata.StyleTransferInputSchema(many=False)
styleGanResultSchema = StyleGanSchemata.StyleTransferOutputSchema(many=False)
StyleGAN = Blueprint('StyleGAN', __name__)

@StyleGAN.route('/inference/style-transfer/', methods=['POST'])
def style_transfer():
    json_data = request.get_json()
    if not json_data:
        return jsonify({'message': 'No input data provided'}), 400

    try:
        deserialized = styleGanInputSchema.load(json_data)
    except ValidationError as error:
        return jsonify(error), 422

    inputBitmap = deserialized.inputImage
    blurred = inputBitmap.filter(ImageFilter.BoxBlur(5))

    buffered = BytesIO()
    blurred.save(buffered, format="BMP")
    encoded_string = base64.b64encode(buffered.getvalue())

    result = StyleGanSchemata.StyleTransferOutput(encoded_string)

    pprint(result)
    
    serialized = styleGanResultSchema.dump(result)
    return jsonify(serialized)
