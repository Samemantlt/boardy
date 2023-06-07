import {observer, useLocalStore} from "mobx-react-lite";
import React from "react";
import styles from "./CreateRoomPage.module.scss";
import {Button, Card, Checkbox, Form, Input, TimePicker} from "antd";
import {TimeoutOptions} from "../../logic/models/events";
import 'dayjs';
import * as dayjs from "dayjs";
import game from "../../logic/models/room";
import {useNavigate} from "react-router-dom";


const format = 'HH:mm:ss';

const CreateRoomPage = observer(() => {
    const navigate = useNavigate();

    const timeoutOptions = useLocalStore(() => ({
        newRoundTimeout: dayjs("00:00:30", format),
        showSecretTimeout: dayjs("00:00:90", format),
        votingTimeout: dayjs("00:00:15", format),
        showRoundResultTimeout: dayjs("00:00:90", format),
        endGameTimeout: dayjs("00:00:15", format)
    }));

    const otherOptions = useLocalStore(() => ({
        roomName: `Room${Math.ceil(Math.random() * 10_000)}`,
        isPublic: true
    }));

    async function createRoom() {
        const converted: TimeoutOptions = {
            newRoundTimeout: timeoutOptions.newRoundTimeout.format(format),
            showSecretTimeout: timeoutOptions.showSecretTimeout.format(format),
            votingTimeout: timeoutOptions.votingTimeout.format(format),
            showRoundResultTimeout: timeoutOptions.showRoundResultTimeout.format(format),
            endGameTimeout: timeoutOptions.endGameTimeout.format(format)
        }

        try {
            await game.createRoom(otherOptions.roomName, converted, otherOptions.isPublic);
            navigate('/');
        } catch (e) {
            alert(`Error: ${e}`)
        }
    }


    return <div className={styles.cardContainer}>
        <Card className={styles.centeredCard}>
            <Form labelWrap={true}
                  labelCol={{span: 16}}
                  wrapperCol={{span: 8}}>
                <Form.Item label="Название комнаты">
                    <Input onInput={(e) => otherOptions.roomName = e.currentTarget.value}
                           value={otherOptions.roomName}/>
                </Form.Item>
                <Form.Item label="Ожидание в начале раунда">
                    <TimePicker onChange={(v, _) => timeoutOptions.newRoundTimeout = v}
                                value={timeoutOptions.newRoundTimeout}/>
                </Form.Item>

                <Form.Item label="Ожидание после раскрытия секрета">
                    <TimePicker onChange={(v, _) => timeoutOptions.showSecretTimeout = v}
                                value={timeoutOptions.showSecretTimeout}/>
                </Form.Item>

                <Form.Item label="Ожидание следующего голоса при голосовании">
                    <TimePicker onChange={(v, _) => timeoutOptions.votingTimeout = v}
                                value={timeoutOptions.votingTimeout}/>
                </Form.Item>

                <Form.Item label="Ожидание после результатов голосования">
                    <TimePicker onChange={(v, _) => timeoutOptions.showRoundResultTimeout = v}
                                value={timeoutOptions.showRoundResultTimeout}/>
                </Form.Item>

                <Form.Item label="Ожидание после окончания игры">
                    <TimePicker onChange={(v, _) => timeoutOptions.endGameTimeout = v}
                                value={timeoutOptions.endGameTimeout}/>
                </Form.Item>

                <Form.Item label="Публичная комната">
                    <Checkbox onChange={(e) => otherOptions.isPublic = e.target.checked}
                              defaultChecked={true}
                              checked={otherOptions.isPublic}/>
                </Form.Item>

                <Form.Item wrapperCol={{offset: 10, span: 5}}>
                    <Button onClick={createRoom}>Создать</Button>
                </Form.Item>
            </Form>
        </Card>
    </div>
});


export default CreateRoomPage;