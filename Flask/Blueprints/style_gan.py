from flask import Flask, jsonify, request, Blueprint
import Schemas.style_gan_schema as StyleGanSchemata
from pprint import pprint
from .Misc import transform
from .Misc import vgg
from .Misc import utils
from .Misc import optimize
import numpy as np
import pdb, os
from PIL import Image, ImageFilter
from io import BytesIO, StringIO
import base64
import tensorflow as tf
import imageio

styleGanInputSchema = StyleGanSchemata.StyleTransferInputSchema(many=False)
styleGanResultSchema = StyleGanSchemata.StyleTransferOutputSchema(many=False)
StyleGAN = Blueprint('StyleGAN', __name__)

device_t='/gpu:0'
soft_config = tf.compat.v1.ConfigProto(allow_soft_placement=True)
soft_config.gpu_options.allow_growth = True
BATCH_SIZE = 1
checkpoint_dir = ".\\Checkpoints"

@StyleGAN.route('/inference/style-transfer/', methods=['POST'])
def style_transfer():
    print("[POST] /inference/style-transfer/")
    json_data = request.get_json()
    if not json_data:
        return jsonify({'message': 'No input data provided'}), 400

    try:
        deserialized = styleGanInputSchema.load(json_data)
    except ValidationError as error:
        return jsonify(error), 422

    inputBitmap = np.asarray(deserialized.inputImage)
    #blurred = inputBitmap.filter(ImageFilter.BoxBlur(5))
    inputBitmap = np.float32(inputBitmap)[:,:,:3]
    # buffered = BytesIO()
    # blurred.save(buffered, format="BMP")
    # encoded_string = base64.b64encode(buffered.getvalue())

    # result = StyleGanSchemata.StyleTransferOutput(encoded_string)
    g = tf.Graph()
    with g.as_default(), g.device(device_t), tf.compat.v1.Session(config=soft_config) as sess:
        img_shape = inputBitmap.shape
        batch_shape = (BATCH_SIZE,) + img_shape
        print("batch_shape:", batch_shape)
        img_placeholder = tf.compat.v1.placeholder(tf.float32, shape=batch_shape, name="img_placeholder")
        print("Constructing placeholder...")
        preds = transform.net(img_placeholder)

        #modelDir = checkpoint_dir + "\\StyleTransfer"
        checkpoint_dir = "C:\\Users\\spigach\\Downloads\\la_muse.ckpt"
        #ckpt = tf.train.get_checkpoint_state(modelDir, latest_filename="rain_princess.ckpt.data-00000-of-00001")
        
        saver= tf.compat.v1.train.Saver()

        if os.path.isdir(checkpoint_dir):
            ckpt = tf.train.get_checkpoint_state(checkpoint_dir)
            if ckpt and ckpt.model_checkpoint_path:
                saver.restore(sess, ckpt.model_checkpoint_path)
            else:
                raise Exception("No checkpoint found...")
        else:
            saver.restore(sess, checkpoint_dir)

        # ckpt = tf.train.latest_checkpoint(modelDir)

        
        print("Loading checkpoint...")
        saver.restore(sess, checkpoint_dir)

        X = np.zeros(batch_shape, dtype=np.float32)
        X[0] = inputBitmap
        print("Running prediction...")
        _preds = sess.run(preds, feed_dict={img_placeholder:X})

        #pprint(_preds)
        print("_preds Shape", _preds.shape)
        
        #preds = np.float32(_preds)[0,:,:,:]
        #print("preds Shape", preds.shape)
        preds = np.squeeze(_preds, axis=0)
        print("preds Shape", preds.shape)

        resImg = np.clip(preds, 0, 255).astype(np.uint8)
        print("resImg Shape", resImg.shape)
        resImg = Image.fromarray(resImg)


        buffered = BytesIO()
        #imageio.imwrite(buffered, resImg)
        resImg.save(buffered, format="BMP")
        result = base64.b64encode(buffered.getvalue()).decode('utf-8')

    #print("RESULT", result)
    #serialized = styleGanResultSchema.dump(result)
    
    return jsonify({"result" : result})
