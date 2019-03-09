import cv2
import os
import numpy as np
import time
import torch

from fastai.vision import *

'''
Note: This should be run from the neural-network folder
'''

def create_learner(model_dir, model_fn):
    '''
    Load the model from disk
    '''
    return load_learner(model_dir, model_fn)

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

def display_point(img_fn, point):
    '''
    Uses OpenCV Functions to draw the nose dot on the image
    '''

    # Draw the point
    img = cv2.imread(img_fn)
    cv2.circle(img, point, 2, (0,0,255), -1)

    # Display the image
    cv2.imshow('labeled nose', img)
    cv2.waitKey(0)
    cv2.destroyAllWindows()

def process_video(learn):
    '''
    Process input from webcam
    '''
    cap = cv2.VideoCapture(0)
    pts = []
    tm = time.time()

    total = 0
    reps = 0

    while(True):
        # Capture frame-by-frame
        ret, frame = cap.read()

        frame = cv2.resize(frame, (160, 120))

        # Process the frame
        point = predict_img(learn, frame)
        
        # Add the point to our list
        pts.append(point)
        if (len(pts) > 10):
            pts = pts[1:]

        # Draw the last 10 points
        for i in range(len(pts) - 1):
            cv2.line(frame, pts[i], pts[i+1], (0,0,255), 2)

        # Display the resulting frame
        cv2.imshow('frame',frame)

        if cv2.waitKey(1) & 0xFF == ord('q'):
            print('Average FPS:', reps / total)
            break

        reps += 1
        total += time.time() - tm
        tm = time.time()

    # When everything done, release the capture
    cap.release()
    cv2.destroyAllWindows()

if __name__ == '__main__':

    learn = create_learner('models', 'vanilla_model.pkl')
    process_video(learn)
    
    
    # Then to display one point, run this
    # img_fn = 'data/nandeeka_test.png'
    # nose_pt = predict_img(learn, cv2.imread(img_fn))
    # display_point(img_fn, nose_pt)