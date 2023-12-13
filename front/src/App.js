import './App.css'
import Navigation from "./Navigation";
import {BrowserRouter, Route, Routes} from "react-router-dom";

function App() {
  return (
    <div className="App">
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Navigation/>}/>
            </Routes>
        </BrowserRouter>
    </div>
  );
}

export default App;
