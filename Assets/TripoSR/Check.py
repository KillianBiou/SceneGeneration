import sys
try:
    import os
    print("All good")
except ImportError as e:
    sys.exit(-1)
sys.exit(0)