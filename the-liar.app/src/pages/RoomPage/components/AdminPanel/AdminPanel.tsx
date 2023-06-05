import {observer} from "mobx-react-lite";
import React from "react";
import styles from './AdminPanel.module.scss'
import {Button, Card, Space, Tooltip} from "antd";
import game from "logic/models/room";

async function copyToClipboard(textToCopy) {
    // Navigator clipboard api needs a secure context (https)
    if (navigator.clipboard && window.isSecureContext) {
        await navigator.clipboard.writeText(textToCopy);
    } else {
        // Use the 'out of viewport hidden text area' trick
        const textArea = document.createElement("textarea");
        textArea.value = textToCopy;

        // Move textarea out of the viewport, so it's not visible
        textArea.style.position = "absolute";
        textArea.style.left = "-999999px";

        document.body.prepend(textArea);
        textArea.select();

        try {
            document.execCommand('copy');
        } catch (error) {
            console.error(error);
        } finally {
            textArea.remove();
        }
    }
}


const AdminPanel = observer(() => {
    async function onClickToCopy(){
        await copyToClipboard(game.room.id);
    }


    return <Card className={styles.adminPanel}>
        <Space direction="vertical">
            <Tooltip title="Нажмите, чтобы скопировать">
                <h4 className={styles.copyTooltip} onClick={onClickToCopy}>Комната: {game.room.id}</h4>
            </Tooltip>
            <Button className={styles.adminButton} onClick={game.nextState}>Next</Button>
        </Space>
    </Card>
});

export default AdminPanel;