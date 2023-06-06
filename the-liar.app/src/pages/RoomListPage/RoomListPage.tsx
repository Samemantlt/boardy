import {observer} from "mobx-react-lite";
import React, {useState} from "react";
import {Button, Card, Table} from "antd";
import styles from './RoomListPage.module.scss';
import roomsList from "../../logic/models/roomsList";
import useAsyncEffect from "use-async-effect";
import {Input} from "antd/lib";
import {useMedia} from 'react-media';
import game from "../../logic/models/room";
import {useNavigate } from "react-router-dom";
import useTime from "../../hooks/useTime";




const RoomListPage = observer(() => {
    const navigate = useNavigate();
    const time = useTime(2_000);

    // TODO: Listen media queries in runtime
    const isSmallScreen = useMedia({query: "(max-width: 1000px)"});


    useAsyncEffect(async () => {
        await roomsList.refresh();
    }, [time]);


    const [roomId, setRoomId] = useState<string>('');


    async function onRowConnectClick(id: string){
        setRoomId(id);
        try {
            await game.joinRoom(id);
            navigate('/');
        } catch (e) {
            alert(`Error: ${e}`)
        }
    }

    const columns = [
        {
            title: 'ID',
            dataIndex: 'id',
            key: 'id',
        },
        {
            title: 'Админ',
            dataIndex: 'adminName',
            key: 'adminName',
        },
        {
            title: 'Игроки',
            dataIndex: 'playersCount',
            key: 'playersCount',
        },
        {
            title: 'Создано',
            dataIndex: 'created',
            key: 'created',
        },
        {
            title: 'Кнопки',
            key: 'actions',
            render: (_, record) => (
                <a onClick={() => onRowConnectClick(record.id)}>Подключиться</a>
            )
        }
    ];

    async function joinRoom() {
        try {
            await game.joinRoom(roomId);
            navigate('/');
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
                        <Button onClick={() => navigate('/rooms/create')}>Создать</Button>
                        <Button onClick={joinRoom}>Подключиться</Button>
                    </div>
                </div>
            </div>
        </Card>
    </div>
});

export default RoomListPage;