import { useState } from 'react';

import "./style/style.css";

const Home = () => {

    const [inputValue, setInputValue] = useState('');
    const [error, setError] = useState('');
    const [result, setResult] = useState('');
    const [prompt, setPrompt] = useState('');
    const [jresult, setJresult] = useState('');

    const handleSubmit = async (event) => {
        event.preventDefault();

        if (!inputValue) {
            setError('Please enter a prompt!');
            setPrompt('');
            setResult('');
            setJresult('');
            return;
        }

        try {
            const response = await fetch('http://localhost:3000/api/chatcompletion', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ text: inputValue })
            });

            if (response.ok) {
                const data = await response.json();
                console.log(data);
                setPrompt(inputValue);
                setResult(data.data.choices[0].text);
                setJresult(JSON.stringify(data.data, null, 2));
                setInputValue('');
                setError('');
            } else {
                throw new Error('An error occured');
            }

        } catch (error) {
            console.log(error);
            setResult('');
            setError('An error occured while submitting the form.');
        }
    };

    return (
        <div className='container'>
            <form className='form-horizontal' onSubmit={handleSubmit}>
                <div className='row form-group mt-1'>
                    <div className='col-sm-10'>
                        <div className='form-floating'>
                            <textarea className='form-control custom-input' id='floatingInput'
                                placeholder='Enter a prompt' value={inputValue} onChange={e => setInputValue(e.target.value)} />
                            <label htmlFor='floatingInput'>Input</label>
                        </div>
                    </div>
                    <div className='col-sm-2'>
                        <button type="submit" className='btn btn-primary custom-button'>Submit</button>
                    </div>
                </div>
            </form>
            {error && <div className='alert alert-danger mt-2 text-start'>{error}</div>}
            {prompt && <div className='alert alert-secondary mt-2 text-start'>{prompt}</div>}
            {result && <div className='alert alert-success mt-2 text-start'>{result}</div>}
            {result && (<pre className='alert alert-info mt-2 text-start' style={{ maxHeight: '320px', overflowY: 'auto' }}><code>{jresult}</code></pre>)}
        </div>
    );
};

export default Home;