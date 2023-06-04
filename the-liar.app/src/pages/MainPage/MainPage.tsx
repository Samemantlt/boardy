import React, {useState} from "react";
import {Button, Input, Space} from "antd";
import './MainPage.scss'
import {observer} from "mobx-react-lite";
import game from "logic/models/room";


const CreateRoomForm = observer(({userName}: {
    userName: string
}) => {
    const [roomName, setRoomName] = useState<string>(null!);

    async function joinRoom() {
        try {
            await game.joinRoom(userName, roomName);
        } catch (e) {
            alert(`Error: ${e}`)
        }
    }

    return <form>
        <Input value={roomName} placeholder="ID группы" onInput={v => setRoomName(v.currentTarget.value)}/>
        <Button type="default" onClick={(e) => joinRoom()}>Присоединиться к комнате</Button>
    </form>
});

const MainPage = observer(() => {
    const [name, setName] = useState(`Player${Math.ceil(Math.random() * 10000)}`)


    return <article>
        <div className="centered">
            <Space>
                <Input value={name} placeholder="Имя" onInput={v => setName(v.currentTarget.value)}/>
                <Button onClick={(e) => game.createRoom(name)}>Создать комнату</Button>
                <CreateRoomForm userName={name}/>
            </Space>
        </div>
    </article>
});

export default MainPage;