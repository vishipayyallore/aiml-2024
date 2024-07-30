# AI 102 - Sample Programs

Some description

## Few Commands

### Analyze Images

```powershell
python -m venv .venv
.\.venv\Scripts\activate

python.exe -m pip install --upgrade pip

pip install python-dotenv
pip install Pillow
pip install matplotlib
py image-analysis.py images/street.jpg
```

### OCR

```powershell
python -m venv .venv
.\.venv\Scripts\activate

python.exe -m pip install --upgrade pip

pip install python-dotenv
pip install Pillow
pip install matplotlib
py read-text.py
```

### Explanation of the Lines

1. **`from PIL import Image, ImageDraw`**

   - **PIL (Python Imaging Library)**: This library provides extensive file format support, an efficient internal representation, and powerful image processing capabilities. It is now maintained under the name Pillow.
     - **`Image`**: This module provides a class with methods for opening, manipulating, and saving many different image file formats.
     - **`ImageDraw`**: This module provides simple 2D graphics support for Image objects. You can use it to draw shapes like rectangles, ellipses, and lines or to add text to images.

2. **`from matplotlib import pyplot as plt`**

   - **Matplotlib**: This is a plotting library for the Python programming language and its numerical mathematics extension NumPy. It provides an object-oriented API for embedding plots into applications.
     - **`pyplot`**: This module provides a MATLAB-like interface. It is used for creating static, interactive, and animated visualizations in Python. With `pyplot`, you can create figures, plots, and customize their appearance easily.

### Usage in the Code

- **PIL**:

  - **`Image`**: Used to open and manipulate images.
  - **`ImageDraw`**: Used to draw bounding boxes and annotations on images based on the analysis results.

- **Matplotlib**:
  - **`pyplot`**: Used to display and save the annotated images with bounding boxes and labels. It handles figure creation, layout adjustments, and saving the final output to a file.
