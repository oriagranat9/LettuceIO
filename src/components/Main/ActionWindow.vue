<template>
    <div>
        <Tabs @delete="terminateTab"/>
        <div v-if="$store.getters.getCurrentTab !== undefined">
            <Connection @action="Action"/>
            <Settings/>
            <component :is="$store.getters.getCurrentTab.actionType" :progress-values="parsedStatus"/>
        </div>
    </div>
</template>

<script>
    import Tabs from "./Tabs";
    import Connection from "./ActionWindow/Connection";
    import Settings from "./ActionWindow/Settings";
    import Record from "./ActionWindow/Actions/Record";
    import Publish from "./ActionWindow/Actions/Publish";

    export default {
        name: "ActionWindow",
        components: {
            Tabs,
            Connection,
            Settings,
            Record,
            Publish
        },
        methods: {
            Action() {
                if (!this.$store.getters.getCurrentTab.status.isActive) {
                    this.startAction();
                } else {
                    const id = this.$store.getters.getCurrentTab['id'];
                    this.terminateAction(id);
                }
            },
            startAction() {
                const self = this;
                // eslint-disable-next-line no-unused-vars
                const {name, tmpLists, status, ...sendDetails} = this.$store.getters.getCurrentTab;
                const selectedTab = this.$store.getters.getCurrentTab;
                this.$set(selectedTab['status'], "isLoading", true);
                this.$ipc.invoke("NewAction", sendDetails).then(state => {
                    if (state) {
                        //setting up the status as running
                        selectedTab['status'] = {
                            isActive: true
                        };

                        self.$ipc.on(sendDetails['id'], (event, message) => {
                            for (let key in message) {
                                if (Object.prototype.hasOwnProperty.call(message, key)) {
                                    this.$set(selectedTab['status'], key, message[key])
                                }
                            }
                            if (!selectedTab['status']['isActive']) {
                                self.$ipc.removeAllListeners(sendDetails['id']);
                            } else {
                                this.$set(selectedTab['status'], "isLoading", false);
                            }
                        })
                    }
                    //   TODO: what happens if task start wasn't successful?
                });
            },
            async terminateAction(id) {
                const self = this;
                return await this.$ipc.invoke("TerminateAction", id).then(state => {
                    if (state) {
                        self.$ipc.removeAllListeners(id);
                        return true;
                    } else {
                        return false;
                    }
                })
            },
            terminateTab(index) {
                const tab = this.$store.getters.tabs[index];
                this.terminateAction(tab['id']).then(state => {
                    if (state) {
                        this.$store.commit('deleteTab', index)
                    }
                });
            }
        },
        computed: {
            parsedStatus: {
                get() {
                    const currentTabStatus = this.$store.getters.getCurrentTab.status;
                    const currentTabLimits = this.$store.getters.getCurrentTab.actionDetails;
                    let messagePercent = 0, sizePercent = 0, secondsPercent = 0;
                    if (currentTabLimits['countLimit'].status) {
                        messagePercent = (currentTabStatus['Count'] / currentTabLimits['countLimit'].value) * 100;
                    }
                    if (currentTabLimits['sizeLimit'].status) {
                        sizePercent = (currentTabStatus['SizeKB'] / currentTabLimits['sizeLimit'].value) * 100;
                    }
                    if (currentTabLimits['timeLimit'].status) {
                        let durationList = currentTabStatus['Duration'].split(":");
                        let secondsPassed = (+durationList[0]) * 60 * 60 + (+durationList[1]) * 60 + (+durationList[2]);
                        secondsPercent = (secondsPassed / currentTabLimits['timeLimit'].value) * 100;
                    }

                    return [messagePercent, sizePercent, secondsPercent]
                }
            }
        }
    }
</script>

<style scoped>
</style>