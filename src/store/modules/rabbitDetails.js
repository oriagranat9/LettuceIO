import {connectionDetails} from "./connectionDetails";
import {recordDetails} from './actionDetails'
import {v4 as uuidv4} from 'uuid';

export function getTemplate() {
    let template = {
        name: "Untitled Action",
        tmpLists: {
            vhosts: [],
            optionList: []
        },
        id: uuidv4(),
        actionType: "Record",
        folderPath: "",
        selectedOption: {
            type: "",
            name: ""
        },
        actionDetails: {...recordDetails},
        connection: {...connectionDetails}
    };

    return {...template}
}