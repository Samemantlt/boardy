import {observer} from "mobx-react-lite";
import React from "react";
import styles from './SecretView.module.scss';
import game from "../../../../logic/models/room";


const SecretView = observer(() => {
    return <>
        <div className={styles.secretView}>
            {game.room.getMainText()}
            <div className={styles.subText}>
                {game.room.getSecondaryText()}
            </div>
        </div>
    </>
})


export default SecretView;