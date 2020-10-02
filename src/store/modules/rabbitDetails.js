import {connectionDetails} from "./connectionDetails";
import {recordDetails} from './actionDetails'
import {v4 as uuid4} from 'uuid';

export function getTemplate() {
    let template = {
        tmpLists: {
            vhosts: [],
            optionList: []
        },
        id: uuid4(),
        actionType: "Record",
        folderPath: undefined,
        selectedOption: {
            type: "",
            name: ""
        },
        actionDetails: {...recordDetails},
        connection: {...connectionDetails},
        status: {

        }
    };

    return {...template}
}