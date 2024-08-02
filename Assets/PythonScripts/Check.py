import sys
try:
	import numpy
	import PIL
	import einops
	import omegaconf
	import transformers
	import trimesh
	import huggingface_hub
	import gradio
	import diffusers
	import rembg
except ImportError as e:
	sys.exit(-1)
sys.exit(0)