import argparse
import sys

from diffusers import AutoPipelineForText2Image, StableDiffusionControlNetPipeline, UniPCMultistepScheduler
import numpy as np
from PIL import Image
import torch

_DEFAULT_DEVICE = (
    'cuda' if torch.cuda.is_available()
    else 'cpu'
)


class StableHandler:
    def __init__(
        self,
        device=_DEFAULT_DEVICE,
        model='Lykon/dreamshaper-8',
    ):
        
        self.pipe = AutoPipelineForText2Image.from_pretrained(
            model, torch_dtype=torch.float16, variant='fp16',
            safety_checker=None,
        ).to("cuda")

    def GenerateBatch(self, desc: str, steps: int, width: int, height: int, nbImage: int, outputDir: str):
        for nb in range(nbImage):
            self.pipe(
                prompt=f'{desc}',
                negative_prompt='',
                num_inference_steps=steps,
                num_images_per_prompt=1,
                width=width,
                height=height,
            ).images[0].save(f"{outputDir}{nb}.png")

    def Generate(self, desc: str, steps: int, width: int, height: int, outputDir: str):
        self.pipe(
            prompt=f'{desc}',
            negative_prompt='',
            num_inference_steps=steps,
            num_images_per_prompt=1,
            width=width,
            height=height,
        ).images[0].save(f"{outputDir}.png")
            
    
if __name__ == '__main__':
    # Argument parse
    parser = argparse.ArgumentParser()
    parser.add_argument('prompt', help='Prompt')
    parser.add_argument('output_path', help='Path for generated image')
    parser.add_argument('--width', help='Width of generated image', default=512)
    parser.add_argument('--height', help='Height of generated image', default=512)
    parser.add_argument(
        '--image-model',
        help='SD 1.5-based model for texture image gen',
        default='Lykon/dreamshaper-8',
    )
    parser.add_argument('--steps', type=int, default=12)
    parser.add_argument('--nbImages', type=int, default=1)
    parser.add_argument(
        '--device',
        default=_DEFAULT_DEVICE,
        type=str,
        help='Device to prefer. Default: try to auto-detect from platform (CUDA, Metal)'
    )
    args = parser.parse_args()

    stableHandler = StableHandler(args.device, args.image_model)
    if(args.nbImages == 1):
        stableHandler.Generate(args.prompt, args.steps, args.width, args.height, args.output_path)
    else:
        stableHandler.GenerateBatch(args.prompt, args.steps, args.width, args.height, args.nbImages, args.output_path)