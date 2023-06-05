import {observer} from "mobx-react-lite";
import React from "react";
import {TimerInfo} from "logic/models/room";
import useTime from "hooks/useTime";
import styles from './Timer.module.scss'

const Timer = observer(({timerInfo}: {
    timerInfo: TimerInfo | undefined
}) => {
    useTime();

    if (timerInfo == null)
        return null;

    let alreadyTaken = Date.now() - timerInfo.started;
    let elapsedSec = Math.ceil(timerInfo.durationSec - alreadyTaken / 1000);

    return <div className={styles.timer}>
        {elapsedSec > 0 ? elapsedSec : 0} сек
    </div>
});

export default Timer;
