import React, {useState} from "react";
import {Button, Card, Form, Input, Space} from "antd";
import './MainPage.scss'
import {observer} from "mobx-react-lite";
import game from "logic/models/room";
import RoomListPage from "../RoomListPage/RoomListPage";


const MainPage = observer(() => {
    const [name, setName] = useState(`Player${Math.ceil(Math.random() * 10000)}`)
    const [continued, setContinued] = useState(false);


    function onContinueBtnClick() {
        game.setName(name);
        setContinued(true);
    }


    if (continued)
        return <RoomListPage/>

    return <article>
        <div className="content">
            <Card>
                <Space direction="vertical">
                    <h1>The Liar</h1>
                    <Input value={name} placeholder="Имя" onInput={v => setName(v.currentTarget.value)}/>
                    <Button onClick={onContinueBtnClick}>Продолжить</Button>
                </Space>
            </Card>
        </div>
    </article>
});

export default MainPage;