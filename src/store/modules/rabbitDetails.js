import {connectionDetails} from "./connectionDetails";
import {recordDetails} from './actionDetails'
import {v4 as uuidv4} from 'uuid';

export function getTemplate() {
    let template = {
        name: "Untitled Action",
        id: uuidv4(),
        actionType: "Record",
        actionDetails: {...recordDetails},
        connection: {...connectionDetails}
    };

    return {...template}
}