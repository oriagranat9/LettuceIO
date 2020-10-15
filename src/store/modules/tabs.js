import {getTemplate} from "./rabbitDetails";
import {publishDetails, recordDetails} from './actionDetails'

export default {
    state: {
        tabs: [],
        selectedTabIndex: 0
    },
    mutations: {
        setTabs(state, tabs) {
            state.tabs = tabs;
        },
        changeTabIndex(state, index) {
            state.selectedTabIndex = index;
        },
        changeTabById(state, id) {
            let index = state.tabs.findIndex(tab => tab.id === id);
            if (index <= 0) {
                state.selectedTabIndex = index;
            }
        },
        deleteTab(state, index) {
            state.tabs.splice(index, 1);
            if (state.selectedTabIndex >= index && state.selectedTabIndex !== 0) {
                state.selectedTabIndex--;
            }
        },
        createTab(state) {
            state.tabs.push(getTemplate());
        },
        setTabValue(state, {key, value}) {
            state.tabs[state.selectedTabIndex][key] = value
        },
        changeActionDetails(state, value) {
            const currentTab = state.tabs[state.selectedTabIndex];
            if (value !== currentTab.actionType){
                currentTab.actionType = value;
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