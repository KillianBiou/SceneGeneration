import argparse
import os
import rembg
import numpy as np

from tsr.utils import remove_background, resize_foreground, save_video
from PIL import Image

if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument("image", type=str, nargs="+", help="Path to input image(s).")
    args = parser.parse_args()

    rembg_session = rembg.new_session()

    images = []

    for i, image_path in enumerate(args.image):
        image = remove_background(Image.open(image_path), rembg_session)
        image = resize_foreground(image, .85)
        image = np.array(image).astype(np.float32) / 255.0
        image = image[:, :, :3] * image[:, :, 3:4] + (1 - image[:, :, 3:4]) * 0.5
        image = Image.fromarray((image * 255.0).astype(np.uint8))
        image.save(image_path)
        print("Image " + str(i) + " finished processing.")
    print("Exiting application")