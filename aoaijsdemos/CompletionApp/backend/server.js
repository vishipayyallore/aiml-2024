const express = require('express');
const dotenv = require('dotenv');
const app = express();

app.use(express.json());

dotenv.config();

const { Configuration, OpenAIApi } = require('openai');

const configuration = new Configuration({
    apiKey: process.env.OPENAI_API_KEY
});

const openai = new OpenAIApi(configuration);

async function runCompletion(prompt) {
    const response = await openai.createCompletion({
        model: "text-davinci-003",
        prompt: prompt,
        max_tokens: 50
    });

    return response;
}

app.post('/api/chatgpt', async (req, res) => {
    try {
        const { text } = req.body;

        const completion = await runCompletion(text);

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
