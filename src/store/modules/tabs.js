import {getTemplate} from "./rabbitDetails";
import {recordDetails, publishDetails} from './actionDetails'

export default {
    state: {
        tabs: [],
        selectedTabIndex: 0
    },
    mutations: {
        setTab(state, tabs) {
            state.tabs = tabs;
        },
        changeTabIndex(state, index) {
            state.selectedTabIndex = index
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