import os
from dotenv import load_dotenv
from openai import AzureOpenAI


def main():

    try:

        load_dotenv()

        azure_oai_endpoint = os.getenv("AZURE_OAI_ENDPOINT")
        azure_oai_key = os.getenv("AZURE_OAI_KEY")
        azure_oai_deployment = os.getenv("AZURE_OAI_DEPLOYMENT")

        client = AzureOpenAI(
            azure_endpoint=azure_oai_endpoint,
            api_key=azure_oai_key,
            api_version="2024-02-15-preview"
        )

        print("Azure OpenAI Chat Completions Example\n")
        print("Starting a chat completion request... Please wait ....\n")

        response = client.chat.completions.create(
            model=azure_oai_deployment,
            messages=[
                {"role": "system", "content": "You are an Expert Cardiologist."},
                {"role": "user", "content": "What is TV?"}
            ]
        )
        generated_text = response.choices[0].message.content

        # Print the response
        print("Response: " + generated_text + "\n")

    except Exception as ex:
        print(ex)


if __name__ == '__main__':
    main()
