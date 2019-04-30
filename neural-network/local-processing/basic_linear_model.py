import numpy as np
import matplotlib.pyplot as plt
import pandas as pd
from time import sleep
import os

import cv2 as cv

IMAGE_HEIGHT = 96
IMAGE_WIDTH = 96

df = pd.read_csv(r"F:\HMC\Junior\Spring\Software Development\facial_keypoints_detection\facial_keypoints_detection\training.csv")

image = df["Image"][0]

print(len(image))

print(df.isnull().any().value_counts())
df.fillna(method = 'ffill',inplace = True)
print(df.isnull().any().value_counts())


imag = []
for i in range(0,7049):
    img = df['Image'][i].split(' ')
    img = ['0' if x == '' else x for x in img]
    imag.append(img)



image_list = np.array(imag,dtype = 'float')
X_train = image_list.reshape(-1,96,96)


plt.imshow(X_train[0], cmap='gray')
plt.show()

"""
img = X_train[0]
cv.imshow('img',img)
cv.waitKey(0)
cv.destroyAllWindows()
"""



training = df.drop('Image',axis = 1)

y_train = []
for i in range(0,7049):
    y = training.iloc[i,:]

    y_train.append(y)
y_train = np.array(y_train,dtype = 'float')


"""
from keras.layers import Conv2D,Dropout,Dense,Flatten
from keras.models import Sequential
model = Sequential([Flatten(input_shape=(96,96)),
                         Dense(128, activation="relu"),
                         Dropout(0.1),
                         Dense(64, activation="relu"),
                         Dense(30)
                         ])
model.compile(optimizer='adam', 
              loss='mse',
              metrics=['mae','accuracy'])
model.fit(X_train,y_train,epochs = 250,batch_size = 128,validation_split = 0.2)
predictions = model.predict(X_train)
"""

import sklearn.linear_model
from sklearn.model_selection import train_test_split
from sklearn import metrics 

linearModel = sklearn.linear_model.LinearRegression(n_jobs = -2)

X_linear_df = df[["left_eye_center_x", "left_eye_center_y", "right_eye_center_x", "right_eye_center_y", "mouth_center_top_lip_x", "mouth_center_top_lip_y"]]
y_linear_df = df[["nose_tip_x", "nose_tip_y"]]

X_linear = []
y_linear = []

for i in range(X_linear_df.shape[0]):
    tmp = []
    for col in X_linear_df.columns:
        tmp.append(X_linear_df[col][i])
    X_linear.append(tmp)

for i in range(y_linear_df.shape[0]):
    tmp = []
    for col in y_linear_df.columns:
        tmp.append(y_linear_df[col][i])
    y_linear.append(tmp)

linearModel.fit(X_linear, y_linear)

print("The linear model's coefficents are", linearModel.coef_)

pred = linearModel.predict(X_linear)
train_mae = sklearn.metrics.mean_absolute_error(y_linear,pred)
train_mse = sklearn.metrics.mean_squared_error(y_linear, pred)

print("The linear model's training mean absolute value error is", train_mae)
print("The linear model's training mean squared error is", train_mse)

def show_image(X, Y):
    img = np.copy(X)
    for i in range(0,Y.shape[0],2):
        if 0 < Y[i+1] < IMAGE_HEIGHT and 0 < Y[i] < IMAGE_WIDTH:
            img[int(Y[i+1]),int(Y[i]),0] = 255
    plt.imshow(img[:,:,0])

# Preview results on test data
def show_results(image_index):
    Ypred = model.predict(X_train[image_index:(image_index+1)])
show_image(X_train[image_index], Ypred[0])