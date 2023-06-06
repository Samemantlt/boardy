import {observer} from "mobx-react-lite";
import React, {useState} from "react";
import {Button, Card, Row, Space, Table} from "antd";
import styles from './RoomListPage.module.scss';
import roomsList from "../../logic/models/roomsList";
import useAsyncEffect from "use-async-effect";
import {Input} from "antd/lib";
import Media, {useMedia} from 'react-media';
import game from "../../logic/models/room";


const columns = [
    {
        title: 'ID',
        dataIndex: 'id',
        key: 'id',
    },
    {
        title: 'Admin',
        dataIndex: 'adminName',
        key: 'adminName',
    },
    {
        title: 'Players',
        dataIndex: 'playersCount',
        key: 'playersCount',
    },
];


const RoomListPage = observer(() => {
    useAsyncEffect(async () => {
        await roomsList.refresh();
    }, []);

    const [roomId, setRoomId] = useState<string>('');

    // TODO: Listen media queries in runtime
    const isSmallScreen = useMedia({query: "(max-width: 1000px)"});


    async function joinRoom() {
        try {
            await game.joinRoom(roomId);
        } catch (e) {
            alert(`Error: ${e}`)
        }
    }

    async function createRoom() {
        try {
            await game.createRoom();
        } catch (e) {
            alert(`Error: ${e}`)
        }
    }


    return <div className={styles.cardContainer}>
        <Card className={styles.centeredCard}>
            <div className={styles.cardBody}>
                <Table size={isSmallScreen ? "small" : null}
                       className={styles.cardBodyTable}
                       dataSource={roomsList.rooms}
                       columns={columns}
                />

                <div className={styles.rawConnectForm}>
                    <Input placeholder="ID комнаты" value={roomId}
                           onInput={e => setRoomId(e.currentTarget.value)}/>
                    <div className={styles.buttons}>
                        <Button onClick={createRoom}>Создать</Button>
                        <Button onClick={joinRoom}>Подключиться</Button>
                    </div>
                </div>
            </div>
        </Card>
    </div>
});

export default RoomListPage;