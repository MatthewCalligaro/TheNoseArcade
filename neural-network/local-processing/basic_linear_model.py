import numpy as np
import matplotlib.pyplot as plt
import pandas as pd
from time import sleep
import os
import sklearn.linear_model
from sklearn.model_selection import train_test_split
from sklearn import metrics
from sklearn import model_selection


IMAGE_HEIGHT = 96
IMAGE_WIDTH = 96

df = pd.read_csv(r"F:\HMC\Junior\Spring\Software Development\facial_keypoints_detection\facial_keypoints_detection\training.csv")


xAndYallData = df[["left_eye_center_x", "left_eye_center_y", "right_eye_center_x", "right_eye_center_y", "mouth_center_bottom_lip_x", "mouth_center_bottom_lip_y", "nose_tip_x", "nose_tip_y", "Image"]]

# Remove the Rows with blanks in them.
print(xAndYallData.isnull().any().value_counts())
xAndYallData = xAndYallData.dropna(axis=0, how="any")
print(xAndYallData.isnull().any().value_counts()) # You want this to print false and the appropriate number of columns.

X = xAndYallData[["left_eye_center_x", "left_eye_center_y", "right_eye_center_x", "right_eye_center_y", "mouth_center_bottom_lip_x", "mouth_center_bottom_lip_y"]].values
y = xAndYallData[["nose_tip_x", "nose_tip_y"]].values
Images = xAndYallData["Image"].values

y_trainNX = y[:,0]
y_trainNY = y[:,1] # should be in column 1

num_splits = 10
n_folds = X.shape[0] # Leave one out cross validation
n_trials = 10

# regularizationParameters = np.power(10,np.arange(-5,5, dtype=float))
error_Array = np.zeros((n_folds, X.shape[0]))


# Make the folds in the cut.
KFold = model_selection.KFold(n_folds, shuffle=True)

linearXModel = sklearn.linear_model.LinearRegression(n_jobs=-2)
linearYModel = sklearn.linear_model.LinearRegression(n_jobs=-2)

dictXnoseX = {"X" : X, "y": y_trainNX}
dictXnoseY = {"X" : X, "y": y_trainNY}


def cv_performance(regressor, train_data, kfs) :
    """
    Determine regressor performance across multiple trials using cross-validation
    
    Parameters
    --------------------
        regressor        -- regressor
        train_data -- Data, training data
        kfs        -- array of size n_trials
                      each element is one fold from model_selection.KFold
    
    Returns
    --------------------
        training_scores     -- numpy array of shape (n_trials, n_fold)
                      each element is the (residual) score of one fold in one trial
    """
    
    n_trials = len(kfs)
    n_folds = kfs[0].n_splits
    training_scores = np.zeros((n_trials, n_folds, train_data["X"].shape[0]), dtype = float)
    validation_scores = np.zeros((n_trials, n_folds, train_data["X"].shape[0]), dtype = float)
    modelParametersFull = np.zeros((n_trials, n_folds, train_data["X"].shape[1]), dtype = float)

    # run multiple trials of CV
    for i in range(n_trials):
        training_scores[i], validation_scores[i], modelParametersFull[i] = cv_performance_one_trial(regressor, train_data, kfs[i])

    
    return training_scores, validation_scores, modelParametersFull


def cv_performance_one_trial(regressor, train_data, kf) :
    """
    Compute regressor performance across multiple folds using cross-validation
    
    Parameters
    --------------------
        regressor        -- regressor
        train_data -- Data, training data
        kf         -- model_selection.KFold
    
    Returns
    --------------------
        training     -- numpy array of shape (n_fold, )
                      each element is the (residual) score of one fold
        validation -- numpy array of shape (n_fold, )
                      each element is the (residual) score of one fold
        modelParameters -- a numpy array of shape (n_fold, d)
                        which corresponds to the fit parameters for the linear regression.
        intercepts -- numpy array of the intercepts of the linear fits.
    """
    num_splits = kf.n_splits
    trainingSize = int(train_data["X"].shape[0]*(num_splits-1)/num_splits)
    training = np.zeros((num_splits, trainingSize), dtype = float)
    validation = np.zeros((num_splits, train_data["X"].shape[0]-trainingSize),dtype = float)
    modelParameters = np.zeros((num_splits, train_data["X"].shape[1]), dtype = float)
    intercepts = np.zeros((num_splits), dtype = float)
    # run one trial of CV

    # Get the indices to split by.
    counter = 0
    for train, test in kf.split(train_data["X"]):
        # split the data.
        X_train_CV, y_train_CV = train_data["X"][train], train_data["y"][train]
        X_test_CV, y_test_CV = train_data["X"][test], train_data["y"][test]
        
        # Fit to the training data.
        regressor.fit(X_train_CV, y_train_CV)


        y_pred = regressor.predict(X_test_CV)
        y_pred_train = regressor.predict(X_train_CV)

        training[counter] = y_train_CV-y_pred_train
        validation[counter] = y_test_CV-y_pred
        modelParameters[counter] = regressor.coef_
        intercepts[counter] = regressor.intercept_
        counter += 1
    
    return training, validation, modelParameters, intercepts

training_error_x, validation_error_x, modelParameters_x, XOffset = cv_performance_one_trial(linearXModel, dictXnoseX, KFold)
training_error_y, validation_error_y, modelParameters_y, yOffset = cv_performance_one_trial(linearYModel, dictXnoseY, KFold)

print(training_error_x)
print(validation_error_x)

print()
print(training_error_y)
print(validation_error_y)

# Compute average validation errors. Signed and absolute value error.
avgTrainX = np.mean(training_error_x, axis = 1)
avgTrainY = np.mean(training_error_y, axis = 1)

avgValX = np.mean(validation_error_x, axis=1)
avgValY = np.mean(validation_error_y, axis=1)

avgTrainXAbsolueError = np.mean(np.abs(training_error_x), axis = 1)
avgTrainYAbsolueError = np.mean(np.abs(training_error_y), axis = 1)

avgValXAbsolueError = np.mean(np.abs(validation_error_x), axis=1)
avgValYAbsolueError = np.mean(np.abs(validation_error_y), axis=1)

print("These are the results for the model to learn the x coordinate of the nose.")
print("The best model has an average validation Absolute error of", np.min(avgValXAbsolueError))
print()
bestIndX = np.where(avgValXAbsolueError == np.min(avgValXAbsolueError))
print("The absolute value of the relative position between the predicted and true x coordinate in the training set is", avgTrainXAbsolueError[bestIndX])
print()
print("These are the model parameters that belong to that minimum value\n", modelParameters_x[bestIndX])
print("The best offset term is", XOffset[bestIndX])

print()
print("These are the results for the model to learn the y coordinate of the nose.")
print("The best model has an average validation Absolute error of", np.min(avgValYAbsolueError))
print()
bestIndY = np.where(avgValYAbsolueError == np.min(avgValYAbsolueError))
print("The absolute value of the relative position between the predicted and true y coordinate in the training set is", avgTrainYAbsolueError[bestIndY])
print()
print("These are the model parameters that belong to that minimum value\n", modelParameters_y[bestIndY])
print("The best offset term is", yOffset[bestIndY])

# Time to display the positions of the nose
bestLinearXModel = sklearn.linear_model.LinearRegression(n_jobs=-2)
bestLinearXModel.coef_ = modelParameters_x[bestIndX]
bestLinearXModel.intercept_ = XOffset[bestIndX]


bestLinearYModel = sklearn.linear_model.LinearRegression(n_jobs=-2)
bestLinearYModel.coef_ = modelParameters_y[bestIndY]
bestLinearYModel.intercept_ = yOffset[bestIndY]

x_Nose_pred = bestLinearXModel.predict(X)
y_Nose_pred = bestLinearYModel.predict(X)

# Graph it.
plt.scatter(y_trainNX,y_trainNY, s=5, c="red")
plt.scatter(x_Nose_pred, y_Nose_pred, s=5, c="blue")

plt.title("Nose Position in the Kaggle Facial Keypoints Dataset")
plt.legend(["True", "Predicted"])

plt.xlabel("X coordinate of the nose")
plt.ylabel("Y coordinate of the nose")
plt.show()


# Now graph some representative images.
def show_image(X, Y):
    img = np.copy(X)
    for i in range(0,len(Y),2):
        if 0 < Y[i+1] < IMAGE_HEIGHT and 0 < Y[i] < IMAGE_WIDTH:
            img[int(Y[i+1]),int(Y[i])] = 255
    plt.imshow(img[:,:,0])

# Preview results on test data
def show_results(image_index):
    NoseX = bestLinearXModel.predict([X[image_index]])
    NoseY = bestLinearYModel.predict([X[image_index]])
    print("Your predicted nose position is (x,y)=("+str(float(NoseX))+","+str(float(NoseY))+").")
    Ypred = [float(NoseX), float(NoseY)]
    show_image(Images[image_index], Ypred)

LowestAverageXErrors = training_error_x[bestIndX[0][0],:]
LowestAverageYErrors = training_error_y[bestIndY[0][0],:]

def getImageIndicies(num_images, ErrorList, BestWorst=1):
    """Get the indices of the top num_images. BestWorst is
    a binary value of 1 or -1. 1 indicates the desire for the
    images to be sorted by lowest to highest errors and -1
    visa versa.

    ErrorList is a numpy array of the errors between the predicted point and the true point.
    """ 
    sortErrors = []
    if BestWorst not in [-1, 1]:
        raise ValueError("The parameter BestWorst is expected to be -1 or 1.")
    if num_images <= len(ErrorList):
        sortErrors = np.argsort(ErrorList)[::BestWorst][:num_images]
    else:
        sortErrors = np.argsort(ErrorList)[::BestWorst] # get the indices of all the images in the appropriate order.
    return sortErrors