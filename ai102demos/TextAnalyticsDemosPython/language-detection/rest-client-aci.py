from dotenv import load_dotenv
import os
import http.client
import json


def main():
    global ai_endpoint
    # global ai_key

    try:
        # Get Configuration Settings
        load_dotenv()
        ai_endpoint = os.getenv('AI_SERVICE_ENDPOINT_ACI')
        print('Using AI ACI Endpoint: ', ai_endpoint)
        # ai_key = os.getenv('AI_SERVICE_KEY')

        # Get user input (until they enter "quit")
        userText = ''
        while userText.lower() != 'quit':
            userText = input(
                'Enter some text for Language Detection using Azure AI Services ("quit" to stop)\n')
            if userText.lower() != 'quit':
                GetLanguage(userText)

    except Exception as ex:
        print(ex)


def GetLanguage(text):
    try:
        # Construct the JSON request body (a collection of documents, each with an ID and text)
        jsonBody = {
            "documents": [
                {"id": 1,
                 "text": text}
            ]
        }

        # Let's take a look at the JSON we'll send to the service
        print(json.dumps(jsonBody, indent=2))

        # Make an HTTP request to the REST interface
        uri = ai_endpoint.rstrip('/').replace('http://', '')
        print('Using URI:', uri)
        conn = http.client.HTTPConnection(uri)

        # Add the authentication key to the request header
        headers = {
            'Content-Type': 'application/json',
        }

        # Use the Text Analytics language API
        conn.request("POST", "/text/analytics/v3.0/languages",
                     str(jsonBody).encode('utf-8'), headers)

        # Send the request
        response = conn.getresponse()
        data = response.read().decode("UTF-8")

        # If the call was successful, get the response
        if response.status == 200:

            # Display the JSON response in full (just so we can see it)
            results = json.loads(data)
            print(json.dumps(results, indent=2))

            # Extract the detected language name for each document
            for document in results["documents"]:
                print("\nLanguage:", document["detectedLanguage"]["name"])

        else:
            # Something went wrong, write the whole response
            print('Non 200 Status: ', data)

        conn.close()

    except Exception as ex:
        print('Exception: ', ex)


if __name__ == "__main__":
    main()
