import '../styles/App.css'
import Navigation from "./Navigation";
import {BrowserRouter, Route, Routes} from "react-router-dom";
import Toolbar from "./Toolbar";
import Authorization from "./Authorization";

const BASE_URL = "http://185.251.88.185:8080"

function App() {
  return (
    <div className="App">
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Authorization/>}/>
            </Routes>
        </BrowserRouter>
    </div>
  );
}

export default App;
