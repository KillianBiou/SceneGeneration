import argparse
import sys
import time


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

    def GenerateBatch(self, prompt: str, nprompt: str, hprompt: str, width: int, height: int, steps: int, cfg: int, seed: int, tile: bool, tileX: bool, tileY: bool, nbImage: int, device: str, outputDir: str, filename: str):
        generator = torch.Generator(device="cuda").manual_seed(int(time.time() * 1000) % (2**32)) if seed == -1 else torch.Generator(device="cuda").manual_seed(seed)
        for nb in range(nbImage):
            print("Generating image " + str(nb))
            self.pipe(
                prompt=f'{prompt}',
                negative_prompt=nprompt,
                width=width,
                height=height,
                num_inference_steps=steps,
                cfg=cfg,
                generator = generator,
                num_images_per_prompt=1,
            ).images[0].save(f"{outputDir}/{filename}{str(nb)}.png")
            print("Generated image " + str(nb))

    def Generate(self, prompt: str, nprompt: str, hprompt: str, width: int, height: int, steps: int, cfg: int, seed: int, tile: bool, tileX: bool, tileY: bool, nbImage: int, device: str, outputDir: str, filename: str):
        generator = torch.Generator(device="cuda").manual_seed(int(time.time() * 1000) % (2**32)) if seed == -1 else torch.Generator(device="cuda").manual_seed(seed)
        self.pipe(
            prompt=f'{prompt}',
            negative_prompt=nprompt,
            width=width,
            height=height,
            num_inference_steps=steps,
            cfg=cfg,
            generator = generator,
            num_images_per_prompt=1,
        ).images[0].save(f"{outputDir}/{filename}.png")
            
    
if __name__ == '__main__':
    # Argument parse
    parser = argparse.ArgumentParser()
    # Prompt param
    parser.add_argument('prompt', help='Prompt')
    parser.add_argument('--nprompt', help='Negative prompt', default="")
    parser.add_argument('--hprompt', help='Hidden Prompt', default="")

    # Image param

    parser.add_argument('--width', type=int, help='Width of generated image', default=512)
    parser.add_argument('--height', type=int, help='Height of generated image', default=512)
    parser.add_argument(
        '--image-model',
        help='SD 1.5-based model for texture image gen',
        default='Lykon/dreamshaper-8',
    )
    parser.add_argument('--steps', type=int, default=12)
    parser.add_argument('--cfg', type=int, default=7, help="CFG Scale for image generation")
    parser.add_argument('--seed', type=int, default=-1, help="CFG Scale for image generation")

    # Misc Image param

    parser.add_argument('--tilling', action="store_true", help="If specified, make the image a tillable")
    parser.add_argument('--tileX', action="store_true", help="If specified, make the image a tillable in x")
    parser.add_argument('--tileY', action="store_true", help="If specified, make the image a tillable in y")
    parser.add_argument('--nbImages', type=int, default=1)
    parser.add_argument(
        '--device',
        default=_DEFAULT_DEVICE,
        type=str,
        help='Device to prefer. Default: try to auto-detect from platform (CUDA, Metal)'
    )

    # Image saving param

    parser.add_argument('--output_path', help='Path for generated image', default="")
    parser.add_argument('--file_name', help='File name for generated images (if batch : file_name0, file_name1, etc ...)', default="generatedImage")
    args = parser.parse_args()

    stableHandler = StableHandler(args.device, args.image_model)
    if(args.nbImages == 1):
        stableHandler.Generate(args.prompt, args.nprompt, args.hprompt, args.width, args.height, args.steps, args.cfg, args.seed, args.tilling, args.tileX, args.tileY, args.nbImages, args.device, args.output_path, args.file_name)
    else:
        stableHandler.GenerateBatch(args.prompt, args.nprompt, args.hprompt, args.width, args.height, args.steps, args.cfg, args.seed, args.tilling, args.tileX, args.tileY, args.nbImages, args.device, args.output_path, args.file_name)