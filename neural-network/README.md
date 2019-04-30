# Neural Network Information

## Running Notebooks on Google Colab
1. Go to https://colab.research.google.com/.
2. Switch the the GitHub tab.
3. Paste in the GitHub link for the notebook you want to open and click search.
4. If necessary, select the notebook you want to open.
5. Go to Edit→Notebook Settings and under Hardware Accelerator choose TPU.

## Notebooks
* **vanilla_head_pose_tracking.ipynb**: Used to train the most basic version of the model, taken directly from fast.ai's lesson3-head-pose.ipynb
* **vanilla_tf_keras_model.ipynb**: Used for training a model on the BIWI Kinect dataset, implemented with Tensorflow and Keras
* **tf_keras_gt_db.ipynb**: Used for training a model on the Georgia Tech face database, implemented with Tensorflow and Keras
* **opencv_haarlike_tf_keras.ipynb**: Used for training a model to find the nose on the Kaggle facial keypoints dataset, implemented with Tensorflow and Keras
* **facial_keypoints_tf_keras.ipynb**: Used for training a model to find any subset of the 15 keypoints on the Kaggle facial keypoints dataset, implemented with Tensorflow and Keras
* **keypoints_to_nose_pos.ipynb**: Used for training a model to find any subset of the 15 keypoints on the Kaggle facial keypoints dataset and then using those to compute nose position with an additional fully connected layer, implemented with Tensorflow and Keras

## Models
* **vanilla_model.pkl**: The most basic version of the model, taken directly from fast.ai's lesson3-head-pose.ipynb
* **test_model.h5**: A poorly trained model, used mostly for testing integration
* **one_trained_layer.h5**: A model with the following architecture: MobileNetV2, global average pooling, fully connected layer
* **two_layer.h5**: A model with the following architecture: MobileNetV2, global average pooling, two fully connected layers
* **two_layer_200.h5**: A model with the following architecture: MobileNetV2, global average pooling, two fully connected layers trained on a larger subset of the BIWI Kinect Headpose Dataset
* **unfrozen_200.h5**: A model with the following architecture: MobileNetV2, global average pooling, two fully connected layers trained on a larger subset of the BIWI Kinect Headpose Dataset with the MobileNet layers unfrozen
* **kaggle.h5** Network trained on the Kaggle dataset to recognize nose position, requires OpenCV cascade classifier to run with the webcam
* **eye_mouth.h5** Metwork trained on the Kaggle dataset to recognize eye and mouth position, requires OpenCV cascade classifier to run with the webcam

