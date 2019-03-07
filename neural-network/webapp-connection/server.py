"""
Filename: server.py
"""

import os
import pandas as pd
from flask import Flask, jsonify, request
import torch

from fastai.vision import *

app = Flask(__name__)

@app.route('/predict', methods=['POST'])
def apicall():
    """API Call

    Pandas dataframe (sent as a payload) from API Call
    """
    try:
        test_json = request.get_json()
        test = pd.read_json(test_json, orient='records')
        print(test)

        #To resolve the issue of TypeError: Cannot compare types 'ndarray(dtype=int64)' and 'str'
        test['Dependents'] = [str(x) for x in list(test['Dependents'])]

        #Getting the Loan_IDs separated out
        loan_ids = test['Loan_ID']

    except Exception as e:
        raise e

    clf = '../models/vanilla_model.pkl'

    if test.empty:
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

        """Add the predictions as Series to a new pandas dataframe
                                OR
           Depending on the use-case, the entire test data appended with the new files
        """

        final_predictions = pd.DataFrame(list(zip(coors)))

        """We can be as creative in sending the responses.
           But we need to send the response codes as well.
        """
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
