# Azure Open AI

This folder contains the sample for Azure Open AI.

**Reference(s):**

> 1. <https://learn.microsoft.com/en-gb/azure/ai-services/openai/dall-e-quickstart?pivots=programming-language-python&tabs=dalle3%2Ccommand-line>
> 1. <https://github.com/openai/openai-python>
> 1. <https://github.com/bharaththippireddy/azureopenaidemo>

## Create a .env file and update the Environment Variables

### .env file

```text
COMPLETIONS_MODEL="text-davinci-003-dev-001"
OPENAI_API_BASE="https://<your resource name>.openai.azure.com"
OPENAI_API_VERSION="2022-12-01"
```

### Environment Variables

```powershell
[System.Environment]::SetEnvironmentVariable('OPENAI_API_KEY', 'YourAPIKEY-11x1x111111x1xxx1x111x1x11x11x1x', 'User')
```

## Creating Virtual Environments

```bash
py -0p
python.exe -m pip install --upgrade pip
pip install virtualenv
python -m venv .venv
./.venv/Scripts/activate
pip freeze
deactivate
```

## Install the packages

```bash
pip install openai
pip install python-dotenv
```
