import server from "logic/network/server";
import {makeAutoObservable} from "mobx";
import {GameRoom} from "./events";

export class RoomsList {
    rooms?: GameRoom[];


    constructor() {
        makeAutoObservable(this, undefined, {autoBind: true, deep: true});
    }


    async refresh() {
        this.rooms = await server.getPublicRooms();
    }
}

const roomsList = new RoomsList();
(window as any).roomsList = roomsList;

export default roomsList;
