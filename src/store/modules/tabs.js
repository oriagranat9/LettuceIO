import {getTemplate} from "./rabbitDetails";

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