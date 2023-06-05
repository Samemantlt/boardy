import {observer} from "mobx-react-lite";
import {Card, Space} from "antd";
import styles from "./PlayerList.module.scss";
import game, {GameStateType, Player} from "logic/models/room";
import React from "react";



const PlayerCard = observer(({player}:{
    player: Player
}) => {
    let isVoting = game.room.state?.name === GameStateType.Voting;

    async function onCardClick(){
        await game.room.addVote(player.id);
    }

    return <Card title={player.name + (player == game.room.getLocal() ? '*' : '')} className={styles.playerCard} onClick={isVoting ? onCardClick : undefined} hoverable={isVoting}>

    </Card>
});


const PlayerList = observer(() => <Space className={styles.playerList}>
    {game.room!.players.map(p => <PlayerCard player={p} key={p.id}/>)}
</Space>);

export default PlayerList;