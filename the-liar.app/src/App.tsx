import {observer} from "mobx-react-lite";
import React from "react";
import MainPage from "./pages/MainPage/MainPage";
import game from "./logic/models/room";
import RoomPage from "./pages/RoomPage/RoomPage";


const App = observer(() => {
    if (game.room != null)
    {
        return <RoomPage/>
    }

    return <MainPage/>
});

export default App;