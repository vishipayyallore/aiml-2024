from dotenv import load_dotenv
import os
import sys
from azure.core.exceptions import HttpResponseError
import requests
from PIL import Image, ImageDraw
from matplotlib import pyplot as plt
from azure.ai.vision.imageanalysis import ImageAnalysisClient
from azure.ai.vision.imageanalysis.models import VisualFeatures
from azure.core.credentials import AzureKeyCredential


def main():
    load_dotenv()
    ai_endpoint = os.getenv('AI_SERVICE_ENDPOINT')
    ai_key = os.getenv('AI_SERVICE_KEY')

    image_file = 'images/street.jpg'
    if len(sys.argv) > 1:
        image_file = sys.argv[1]

    try:
        with open(image_file, "rb") as f:
            image_data = f.read()
    except FileNotFoundError:
        print(f"Image file '{image_file}' not found.")
        return

    cv_client = ImageAnalysisClient(
        endpoint=ai_endpoint,
        credential=AzureKeyCredential(ai_key)
    )

    try:
        analyze_image(image_file, image_data, cv_client)
        background_removal(ai_endpoint, ai_key, image_file)
    except Exception as ex:
        print(f"An error occurred: {ex}")


def analyze_image(image_filename: str, image_data: bytes, cv_client: ImageAnalysisClient):
    print('\nAnalyzing image...')

    try:
        result = cv_client.analyze(
            image_data=image_data,
            visual_features=[
                VisualFeatures.CAPTION,
                VisualFeatures.DENSE_CAPTIONS,
                VisualFeatures.TAGS,
                VisualFeatures.OBJECTS,
                VisualFeatures.PEOPLE],
        )
    except HttpResponseError as e:
        print(f"Status code: {e.status_code}")
        print(f"Reason: {e.reason}")
        print(f"Message: {e.error.message}")
        return

    display_analysis_results(result, image_filename)


def display_analysis_results(result, image_filename: str):
    if result.caption:
        print("\nCaption:")
        print(f" Caption: '{result.caption.text}' (confidence: {
              result.caption.confidence * 100:.2f}%)")

    if result.dense_captions:
        print("\nDense Captions:")
        for caption in result.dense_captions.list:
            print(f" Caption: '{caption.text}' (confidence: {
                  caption.confidence * 100:.2f}%)")

    if result.tags:
        print("\nTags:")
        for tag in result.tags.list:
            print(f" Tag: '{tag.name}' (confidence: {
                  tag.confidence * 100:.2f}%)")

    if result.objects:
        annotate_image(result.objects.list, image_filename, 'objects.jpg')

    if result.people:
        annotate_people(result.people.list, image_filename, 'people.jpg')


def annotate_image(detected_items, image_filename: str, outputfile: str):
    image = Image.open(image_filename)
    fig = plt.figure(figsize=(image.width/100, image.height/100))
    plt.axis('off')
    draw = ImageDraw.Draw(image)
    color = 'cyan'

    for item in detected_items:
        name = item.tags[0].name
        print(f" {name} (confidence: {item.tags[0].confidence * 100:.2f}%)")
        r = item.bounding_box
        bounding_box = ((r.x, r.y), (r.x + r.width, r.y + r.height))
        draw.rectangle(bounding_box, outline=color, width=3)
        plt.annotate(name, (r.x, r.y), backgroundcolor=color)

    plt.imshow(image)
    plt.tight_layout(pad=0)
    fig.savefig(outputfile)
    print(f'  Results saved in {outputfile}')


def annotate_people(detected_people, image_filename: str, outputfile: str):
    image = Image.open(image_filename)
    fig = plt.figure(figsize=(image.width/100, image.height/100))
    plt.axis('off')
    draw = ImageDraw.Draw(image)
    color = 'cyan'

    for person in detected_people:
        print(f" Person (confidence: {person.confidence * 100:.2f}%)")
        r = person.bounding_box
        bounding_box = ((r.x, r.y), (r.x + r.width, r.y + r.height))
        draw.rectangle(bounding_box, outline=color, width=3)
        plt.annotate("person", (r.x, r.y), backgroundcolor=color)

    plt.imshow(image)
    plt.tight_layout(pad=0)
    fig.savefig(outputfile)
    print(f'  Results saved in {outputfile}')


def background_removal(endpoint: str, key: str, image_file: str):
    print('\nRemoving background from image...')
    api_version = "2023-02-01-preview"
    mode = "backgroundRemoval"

    url = f"{
        endpoint}computervision/imageanalysis:segment?api-version={api_version}&mode={mode}"
    headers = {
        "Ocp-Apim-Subscription-Key": key,
        "Content-Type": "application/json"
    }

    image_url = f"https://github.com/MicrosoftLearning/mslearn-ai-vision/blob/main/Labfiles/01-analyze-images/Python/image-analysis/{
        image_file}?raw=true"
    body = {"url": image_url}

    try:
        response = requests.post(url, headers=headers, json=body)
        response.raise_for_status()
    except requests.exceptions.RequestException as e:
        print(f"Error removing background: {e}")
        return

    with open("background.png", "wb") as file:
        file.write(response.content)
    print('  Results saved in background.png \n')


if __name__ == "__main__":
    main()
