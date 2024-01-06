const express = require('express');
const dotenv = require('dotenv');
const app = express();

app.use(express.json());

dotenv.config();

const { OpenAIClient, AzureKeyCredential } = require("@azure/openai");
const endpoint = process.env["AZURE_OPENAI_ENDPOINT"];
const azureApiKey = process.env["OPENAI_G4_API_KEY"];

const azOpenAiClient = new OpenAIClient(endpoint, new AzureKeyCredential(azureApiKey));

async function runCompletion(prompt) {
    const deploymentName = process.env["COMPLETIONS_DEPLOYMENT_NAME"];

    const response = await azOpenAiClient.getCompletions(deploymentName, prompt);

    return response;
}

app.get('/', function (req, res) {
    res.send("Welcome to Azure Open AI API")
});

app.post('/api/chatcompletion', async (req, res) => {
    try {
        const { text } = req.body;

        const completion = await runCompletion(text);
        console.log('Completion: ', completion.choices[0].text);

        res.json({ data: completion.choices[0].text });

    } catch (error) {
        if (error.response) {
            console.error(error.response.status, error.response.data);
            res.status(error.response.status).json(error.response.data)
        } else {
            console.error('Error with OPENAI API request:', error.message);
            res.status(500).json({
                error: {
                    message: 'An error occured during your request.'
                }
            })
        }
    }
});

const PORT = process.env.PORT || 3000;

app.listen(PORT, console.log(`Server started on port ${PORT}`));
