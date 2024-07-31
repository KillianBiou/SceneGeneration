@echo off

REM Check if Python path argument is provided
if "%~1"=="" (
    echo No Python path provided.
    pause
    exit /b 1
)

REM Use the provided Python path
set PYTHON_PATH=%~1

REM Install packages from the default PyPI index
%PYTHON_PATH% -m pip install -r requirements.txt

REM Install packages from the PyTorch index
%PYTHON_PATH% -m pip install -r Req2.txt --index-url https://download.pytorch.org/whl/cu121

echo Packages installed successfully
pause