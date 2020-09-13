import {getTemplate} from "./rabbitDetails";
import {recordDetails, publishDetails} from './actionDetails'

export default {
    state: {
        syncRoute: {},
        tabs: [],
        selectedTabIndex: 0
    },
    mutations: {
        setTabs(state, tabs) {
            state.tabs = tabs;
            syncRoute(state);
        },
        changeTabIndex(state, index) {
            state.selectedTabIndex = index
            syncRoute(state);
        },
        deleteTab(state, index) {
            state.tabs.splice(index, 1);
            if (state.selectedTabIndex >= index && state.selectedTabIndex !== 0) {
                state.selectedTabIndex--;
            }
            syncRoute(state);
        },
        createTab(state) {
            state.tabs.push(getTemplate());
            syncRoute(state);
        },
        setTabValue(state, {key, value}) {
            state.tabs[state.selectedTabIndex][key] = value
        },
        changeActionDetails(state) {
            const currentTab = state.tabs[state.selectedTabIndex];
            switch (currentTab.actionType) {
                case "Record":
                    currentTab.actionDetails = {...recordDetails};
                    break;
                case "Publish":
                    currentTab.actionDetails = {...publishDetails};
                    break;
            }
        }
    },
    getters: {
        getTabIndex(state) {
            return state.selectedTabIndex
        },
        getTabs(state) {
            return state.tabs
        },
        getCurrentTab(state) {
            return state.tabs[state.selectedTabIndex]
        }
    }
}

function syncRoute(state) {
    state.syncRoute = {};
    for (let index in state.tabs){
        state.syncRoute[state.tabs[index]['id']] = index
    }
}