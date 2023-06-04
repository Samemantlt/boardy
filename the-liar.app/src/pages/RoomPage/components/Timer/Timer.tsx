import {observer} from "mobx-react-lite";
import React from "react";
import game from "logic/models/room";
import useTime from "hooks/useTime";
import styles from './Timer.module.scss'

const Timer = observer(() => {
    useTime();

    if (game.room.timer == null)
        return null;

    let alreadyTaken = Date.now() - game.room.timer.started;
    let elapsedSec = Math.ceil(game.room.timer.durationSec - alreadyTaken / 1000);

    return <div className={styles.timer}>
        {elapsedSec > 0 ? elapsedSec : 0} сек
    </div>
});

export default Timer;
