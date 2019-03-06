import numpy as np
import torch
import torch.onnx

from fastai.vision import *

def convert_pytorch_to_onnx(model_dir, model_fn, export_fn):
    # Load the model
    learn = load_learner(model_dir, model_fn)
    torch.save(learn,"models/torch_model")
    model = torch.load("models/torch_model")

    # Create dummy input for export
    np_img = np.random.randint(256, size=(160,120,3))
    dummy_input = Image(pil2tensor(np_img, np.float32).div_(255))

    # Export file
    torch.onnx.export(model, dummy_input, export_fn)

if __name__ == '__main__':
    convert_pytorch_to_onnx('models', 'vanilla_model.pkl', 'models/vanilla_model.onnx')