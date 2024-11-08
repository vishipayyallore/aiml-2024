import os
from dotenv import load_dotenv

# Add Azure OpenAI package
from openai import AzureOpenAI


def main():

    try:

        # Get configuration settings
        load_dotenv()
        azure_oai_endpoint = os.getenv("AZURE_OAI_ENDPOINT")
        azure_oai_key = os.getenv("AZURE_OAI_KEY")
        azure_oai_deployment = os.getenv("AZURE_OAI_DEPLOYMENT")

        # Initialize the Azure OpenAI client...
        client = AzureOpenAI(
            azure_endpoint=azure_oai_endpoint,
            api_key=azure_oai_key,
            # Target version of the API, such as 2024-02-15-preview
            api_version="2024-02-15-preview"
        )

        response = client.chat.completions.create(
            model=azure_oai_deployment,
            messages=[
                {"role": "system", "content": "You are a helpful assistant."},
                {"role": "user", "content": "What is Azure OpenAI?"}
            ]
        )
        generated_text = response.choices[0].message.content

        # Print the response
        print("Response: " + generated_text + "\n")

        while True:
            # Get input text
            input_text = input("Enter the prompt (or type 'quit' to exit): ")
            if input_text.lower() == "quit":
                break
            if len(input_text) == 0:
                print("Please enter a prompt.")
                continue

            print("\nSending request for summary to Azure OpenAI endpoint...\n\n")

            # Add code to send request...

    except Exception as ex:
        print(ex)


if __name__ == '__main__':
    main()
