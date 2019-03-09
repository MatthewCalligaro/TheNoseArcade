"""
Filename: server.py
"""
import cv2
import numpy as np
from flask import Flask, jsonify, request
import os
import pandas as pd
import torch

from fastai.vision import *

app = Flask(__name__)

@app.route('/predict', methods=['POST'])
def apicall():
    try:
        # convert string of image data to uint8
        nparr = np.fromstring(request.data, np.uint8)
        # decode image
        img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
    
    except Exception as e:
        raise e

    # Path to the network
    clf = '../models/vanilla_model.pkl'

    # Make sure the image has all three dimensions
    if 0 in nparr.shape:
        return(bad_request())

    else:
        #Load the saved model
        print("Loading the model...")
        loaded_model = None
        with open(clf,'rb') as f:
            loaded_model = load_learner(os.path.split(clf)[0], os.path.split(clf)[1])

        coors = predict_img(loaded_model, test)

        print("The model has been loaded...doing predictions now...")
        predictions = loaded_model.predict(test)

        # Make a dataframe of rhe results
        final_predictions = pd.DataFrame(list(zip(coors)))

        # Send the result and response code
        responses = jsonify(predictions=final_predictions.to_json(orient="records"))
        responses.status_code = 200

        return (responses)

def predict_img(learn, np_img):
    '''
    Predict the head pose of a given numpy array image
    '''
    #img = open_image(img_fn)
    # Load the image in the correct format
    img = Image(pil2tensor(np_img, np.float32).div_(255))

    ips, _, _ = learn.predict(img.apply_tfms(None, size=(120,160)))
    return get_coords(img, ips)

def get_coords(img, ips):
    '''
    Given an image and image point, return the X, Y coordinates of the
    ImagePoints
    '''
    pts = to_np((ips.data[0] + 1) / 2)
    return int(pts[1] * img.size[1]), int(pts[0] * img.size[0])
