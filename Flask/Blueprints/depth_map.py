from flask import Flask, jsonify, request, Blueprint
import Schemas.depth_map_schema as DepthMapSchemata
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
from marshmallow import Schema, fields, ValidationError
import matplotlib.pyplot as plt

from .Misc.monodepth_model import *
from .Misc.monodepth_dataloader import *
from .Misc.average_gradients import *

tf.compat.v1.disable_eager_execution()

depthMapInputSchema = DepthMapSchemata.DepthMapInputSchema(many=False)
DepthMap = Blueprint('DepthMap', __name__)

@DepthMap.route('/inference/depth-map/', methods=['POST'])
def depth_map():
    print("[POST] /inference/depth-map/")

    json_data = request.get_json()

    if not json_data:
        return jsonify({'message': 'No input data provided'}), 400

    try:
        deserialized = depthMapInputSchema.load(json_data)
    except ValidationError as error:
        return jsonify(error), 422

    input_image = deserialized.inputImage
    
    

    
    print("\n\nJUST DECODED")
    pprint(input_image)

    params = {
        "encoder":'vgg',
        "height":256, 
        "width":512, 
        "batch_size":2, 
        "num_threads":1, 
        "num_epochs":1, 
        "do_stereo":False, 
        "wrap_mode":'border', 
        "use_deconv":False, 
        "alpha_image_loss":0, 
        "disp_gradient_loss_weight":0, 
        "lr_loss_weight":0, 
        "full_summary":False
    }

    left  = tf.compat.v1.placeholder(tf.float32, [2, params['height'], params['width'], 3])
    model = MonodepthModel(params, "test", left, None)

    # SESSION
    config = tf.compat.v1.ConfigProto(allow_soft_placement=True)
    sess = tf.compat.v1.Session(config=config)

    # SAVER
    train_saver = tf.compat.v1.train.Saver()

    # INIT
    sess.run(tf.compat.v1.global_variables_initializer())
    sess.run(tf.compat.v1.local_variables_initializer())
    coordinator = tf.compat.v1.train.Coordinator()
    threads = tf.compat.v1.train.start_queue_runners(sess=sess, coord=coordinator)

    # RESTORE
    restore_path = "Checkpoints\\DepthMap\\model_cityscapes".split(".")[0]
    train_saver.restore(sess, restore_path)

    
    input_image = np.array(input_image).astype(np.float)
    original_height, original_width, num_channels = input_image.shape
    print("original_height", original_height)
    print("original_width", original_width)
    print("num_channels", num_channels)
    print("input_image.shape", input_image.shape)
    print("\n\nA BUNCH OF CONVERSIONS")
    pprint(input_image)

    input_image = np.array(Image.fromarray((input_image).astype(np.uint8)).resize((512, 256), resample=Image.BICUBIC))
    input_image = input_image/ 255
    input_image = np.float32(input_image)[:,:,:3]
    input_images = np.stack((input_image, np.fliplr(input_image)), 0)
    

    disp = sess.run(model.disp_left_est[0], feed_dict={left: input_images})
    disp_pp = post_process_disparity(disp.squeeze()).astype(np.float32)

    # disp_to_img = disp_pp.squeeze()
    disp_to_img = disp_pp
    print("\n\ndisp_to_img")
    pprint(disp_to_img)

    dIm = Image.fromarray(disp_to_img)
    s = (original_width, original_height)
    disp_to_img = np.array(dIm.resize(s, Image.BICUBIC))

    #resImg = Image.fromarray(disp_to_img)
    buffered = BytesIO()
    #imageio.imwrite(buffered, resImg)
    # if resImg.mode != 'RGB':
    #     resImg = resImg.convert('RGB')
    # resImg.save(buffered, format="BMP")
    plt.imsave(buffered, disp_to_img, cmap='plasma')
    result = base64.b64encode(buffered.getvalue()).decode('utf-8')

    return jsonify({"result" : result})

def post_process_disparity(disp):
    _, h, w = disp.shape
    l_disp = disp[0,:,:]
    r_disp = np.fliplr(disp[1,:,:])
    m_disp = 0.5 * (l_disp + r_disp)
    l, _ = np.meshgrid(np.linspace(0, 1, w), np.linspace(0, 1, h))
    l_mask = 1.0 - np.clip(20 * (l - 0.05), 0, 1)
    r_mask = np.fliplr(l_mask)
    return r_mask * l_disp + l_mask * r_disp + (1.0 - l_mask - r_mask) * m_disp
