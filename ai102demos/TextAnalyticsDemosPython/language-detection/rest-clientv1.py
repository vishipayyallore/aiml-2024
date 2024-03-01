from dotenv import load_dotenv
import os
import http.client
import json


class AzureLanguageDetection:
    def __init__(self):
        self.load_configuration()

    def load_configuration(self):
        load_dotenv()
        self.ai_endpoint = os.getenv('AI_SERVICE_ENDPOINT')
        self.ai_key = os.getenv('AI_SERVICE_KEY')

    def get_language(self, text):
        try:
            json_body = {
                "documents": [
                    {"id": 1,
                     "text": text}
                ]
            }

            # Display the JSON request body
            self.print_json(json_body)

            # Make an HTTP request to the REST interface
            uri = self.ai_endpoint.rstrip('/').replace('https://', '')
            conn = http.client.HTTPSConnection(uri)

            # Add the authentication key to the request header
            headers = {
                'Content-Type': 'application/json',
                'Ocp-Apim-Subscription-Key': self.ai_key
            }

            # Use the Text Analytics language API
            conn.request("POST", "/text/analytics/v3.1/languages?",
                         json.dumps(json_body).encode('utf-8'), headers)

            # Send the request
            response = conn.getresponse()
            data = response.read().decode("UTF-8")

            # Process the response
            self.process_response(response, data)

            conn.close()

        except Exception as ex:
            print(ex)

    def process_response(self, response, data):
        if response.status == 200:
            # Display the JSON response
            results = json.loads(data)
            self.print_json(results)

            # Extract and display the detected language name for each document
            for document in results["documents"]:
                print("\nLanguage:", document["detectedLanguage"]["name"])
        else:
            # Something went wrong, display the whole response
            print(data)

    @staticmethod
    def print_json(data):
        # Pretty print JSON data for better readability
        print(json.dumps(data, indent=2))


def main():
    try:
        language_detector = AzureLanguageDetection()

        # Get user input (until they enter "quit")
        user_text = ''
        while user_text.lower() != 'quit':
            user_text = input(
                'Enter some text for Language Detection using Azure AI Services ("quit" to stop)\n')
            if user_text.lower() != 'quit':
                language_detector.get_language(user_text)

    except Exception as ex:
        print(ex)


if __name__ == "__main__":
    main()
