import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import Home from './screens/Home'
import './App.css'
import "./styles/bootstrap-custom.scss"
import NavBar from './nav/NavBar';

const App = () => {
  return (
    <>
      <NavBar />
      <div className="container my-5">
        <Router>
          <Routes>
            <Route path="/" element={<Home />} />
          </Routes>
        </Router>
      </div>
    </>
  )
}

export default App;
