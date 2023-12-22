# https://learn.microsoft.com/en-us/azure/cognitive-services/openai/how-to/create-resource?pivots=web-portal#create-a-resource
# Note: The openai-python library support for Azure OpenAI is in preview.
# Note: DALL-E 3 requires version 1.0.0 of the openai-python library or later
from openai import AzureOpenAI
from dotenv import dotenv_values
import json

# Load config values
config_details = dotenv_values(".env")

client = AzureOpenAI(
    api_version=config_details["OPENAI_API_VERSION"],
    azure_deployment=config_details["OPENAI_API_DEPLOYMENT"], # the name of your DALL-E 3 deployment
    azure_endpoint=config_details["OPENAI_API_ENDPOINT"],
    api_key=config_details["OPENAI_API_KEY"],
)

result = client.images.generate(
    model="dall-e-3", # the name of your DALL-E 3 deployment
    prompt="Flag of India",
    n=1
)

image_url = json.loads(result.model_dump_json())['data'][0]['url']
print(image_url)

