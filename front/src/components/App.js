import '../styles/App.css'
import Navigation from "./Navigation";
import {BrowserRouter, Route, Routes} from "react-router-dom";
import Toolbar from "./Toolbar";

function App() {
  return (
    <div className="App">
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Navigation/>}/>
            </Routes>
        </BrowserRouter>

        <Toolbar/>
    </div>
  );
}

export default App;
