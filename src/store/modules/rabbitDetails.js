import {connectionDetails} from "./connectionDetails";
import { v4 as uuidv4 } from 'uuid';

export function getTemplate() {
    let template = {
        name: "Untitled Action",
        id: uuidv4(),
        connection: {...connectionDetails}
    };

    return {...template}
}