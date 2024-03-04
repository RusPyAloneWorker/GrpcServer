import './static/index.css'
import './App.css';
import {Header} from "./Components/Header";
import {BrowserRouter, Route, Routes} from "react-router-dom";
import {Homepage} from "./Pages/Homepage/Homepage";

function App() {
  return (
      <>
      <Header></Header>
      <BrowserRouter>
        <Routes>
          <Route path="" element={<Homepage />} />
        </Routes>
      </BrowserRouter>
    </>
  );
}

export default App;
