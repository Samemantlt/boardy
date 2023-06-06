import {observer} from "mobx-react-lite";
import React from "react";
import MainPage from "./pages/MainPage/MainPage";
import game from "./logic/models/room";
import RoomPage from "./pages/RoomPage/RoomPage";
import {Route, Routes, useNavigate} from "react-router-dom";
import CreateRoomPage from "pages/CreateRoomPage/CreateRoomPage";
import RoomListPage from "pages/RoomListPage/RoomListPage";
import server from "./logic/network/server";


const App = observer(() => {
    const navigate = useNavigate();


    if (!server.connected)
        return <h3>Подключение</h3>


    if (game.room != null)
    {
        return <RoomPage/>
    }
    if (window.location.pathname != '/' && (game.localPlayerName == null || game.localPlayerName == ''))
    {
        navigate('/');
        return null
    }

    return <Routes>
            <Route path="/" element={<MainPage/>}/>
            <Route path="/rooms" element={<RoomListPage/>}/>
            <Route path="/rooms/create" element={<CreateRoomPage/>}/>
        </Routes>
});

export default App;