# Neural Network Information

## Models
* **vanilla_model.pkl**: The most basic version of the model, taken directly from fast.ai's lesson3-head-pose.ipynb
* **test_model.h5**: A poorly trained model, used mostly for testing integration
* **one_trained_layer.h5**: A model with the following architecture: MobileNetV2, global average pooling, fully connected layer
* **two_layer.h5**: A model with the following architecture: MobileNetV2, global average pooling, two fully connected layers
* **two_layer_200.h5**: A model with the following architecture: MobileNetV2, global average pooling, two fully connected layers trained on a larger subset of the BIWI Kinect Headpose Dataset
* **unfrozen_200.h5**: A model with the following architecture: MobileNetV2, global average pooling, two fully connected layers trained on a larger subset of the BIWI Kinect Headpose Dataset with the MobileNet layers unfrozen
* **kaggle.h5** Network trained on the Kaggle dataset to recognize nose position, requires OpenCV cascade classifier to run with the webcam
* **eye_mouth.h5** Metwork trained on the Kaggle dataset to recognize eye and mouth position, requires OpenCV cascade classifier to run with the webcam

## Running Notebooks on Google Colab
1. Go to https://colab.research.google.com/.
2. Switch the the GitHub tab.
3. Paste in the GitHub link for the notebook you want to open and click search.
4. If necessary, select the notebook you want to open.
5. Go to Edit→Notebook Settings and under Hardware Accelerator choose GPU.

