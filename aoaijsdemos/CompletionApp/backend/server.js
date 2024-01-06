const express = require('express');
const dotenv = require('dotenv');
const app = express();

app.use(express.json());

dotenv.config();

// const { Configuration, OpenAIApi } = require('openai');
const { OpenAIClient, AzureKeyCredential } = require("@azure/openai");
const endpoint = process.env["AZURE_OPENAI_ENDPOINT"];
const azureApiKey = process.env["OPENAI_G4_API_KEY"];
// console.log('Azure OpenAI Endpoint: ', endpoint);
// console.log('Azure OpenAI API Key: ', azureApiKey);

const client = new OpenAIClient(endpoint, new AzureKeyCredential(azureApiKey));

// const configuration = new Configuration({
//     apiKey: process.env.OPENAI_G4_API_KEY
// });

// const openai = new OpenAIApi(configuration);

async function runCompletion(prompt) {
    const deploymentName = process.env["COMPLETIONS_DEPLOYMENT_NAME"];
    const response = await client.getCompletions(deploymentName, prompt);

    // console.log('Response Received: ', response);
    // const response = await openai.createCompletion({
    //     model: "text-davinci-003",
    //     prompt: prompt,
    //     max_tokens: 50
    // });

    return response;
}

app.get('/', function (req, res) {
    res.send("Hello world!")
});

app.post('/api/chatgpt', async (req, res) => {
    try {
        const { text } = req.body;

        const completion = await runCompletion(text);
        console.log('Completion: ', completion);

        res.json({ data: completion.data });

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

const PORT = process.env.PORT || 5000;

app.listen(PORT, console.log(`Server started on port ${PORT}`));
