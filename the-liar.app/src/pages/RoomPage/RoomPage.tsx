import {observer} from "mobx-react-lite";
import game from "../../logic/models/room";
import styles from './RoomPage.module.scss';
import React from "react";
import PlayerList from "./components/PlayerList/PlayerList";
import SecretView from "./components/SecretView/SecretView";
import Timer from "./components/Timer/Timer";
import AdminPanel from "./components/AdminPanel/AdminPanel";


const RoomPage = observer(() => {
    if (game.room == null)
        throw new Error('Room must not be null')

    return <div className={styles.roomPage}>
        <SecretView/>
        {game.admin ? <AdminPanel/> : null}
        <Timer/>
        <PlayerList/>
    </div>
});

export default RoomPage;