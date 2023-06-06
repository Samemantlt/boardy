import React, {useState} from "react";
import {Button, Card, Input, Space} from "antd";
import './MainPage.scss'
import {observer} from "mobx-react-lite";
import game from "logic/models/room";
import {useNavigate} from "react-router-dom";


const MainPage = observer(() => {
    const navigate = useNavigate();
    const [name, setName] = useState(`Player${Math.ceil(Math.random() * 10000)}`)


    function onContinueBtnClick() {
        game.setName(name);
        navigate('/rooms');
    }


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