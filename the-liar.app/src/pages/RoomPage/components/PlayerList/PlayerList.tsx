// noinspection HtmlUnknownAttribute

import {observer} from "mobx-react-lite";
import {Card, Space} from "antd";
import styles from "./PlayerList.module.scss";
import game, {GameStateType, Player} from "logic/models/room";
import React from "react";
import {ScrollMenu} from "react-horizontal-scrolling-menu";
import "react-horizontal-scrolling-menu/dist/styles.css"

function onWheel(apiObj: any, ev: React.WheelEvent): void {
    const isThouchpad = Math.abs(ev.deltaX) !== 0 || Math.abs(ev.deltaY) < 15;

    if (isThouchpad) {
        ev.stopPropagation();
        return;
    }

    if (ev.deltaY < 0) {
        apiObj.scrollNext();
    } else if (ev.deltaY > 0) {
        apiObj.scrollPrev();
    }
}


const PlayerCard = observer(({player, ...props}: any & {
    player: Player
}) => {
    let isVoting = game.room.state?.type === GameStateType.Voting;

    async function onCardClick(){
        await game.room.addVote(player.id);
    }

    return <Card {...props} title={player.name + (player == game.room.getLocal() ? '*' : '')} className={styles.playerCard} onClick={isVoting ? onCardClick : undefined} hoverable={isVoting}>

    </Card>
});


const PlayerList = observer(() => {
    // {game.room!.players.map(p => <PlayerCard itemId={p.id} player={p} key={p.id}/>)}
    // {game.room!.players.map(p => <span itemId={p.id} className={styles.playerCard} key={p.id}>{p.name}</span>)}
    // @ts-ignore
    return <div className={styles.playerListWrapper}>
        <ScrollMenu onWheel={onWheel} >
            {game.room!.players.map(p => <PlayerCard itemId={p.id} player={p} key={p.id}/>)}
        </ScrollMenu>
    </div>
});

export default PlayerList;