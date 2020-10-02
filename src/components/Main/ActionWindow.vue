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
    import ExchangePlaceholders from "./ActionWindow/ExchangePlaceholders";

    export default {
        name: "ActionWindow",
        data() {
            return {
                placeholders: ExchangePlaceholders
            }
        },
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
                const {tmpLists, status, ...sendDetails} = this.$store.getters.getCurrentTab;
                const selectedTab = this.$store.getters.getCurrentTab;
                //check if there's no routing key for recording from exchange
                if (sendDetails.actionType === "Record" && sendDetails.selectedOption.type === "Exchange" && sendDetails.actionDetails.bindingRoutingKey === "") {
                    sendDetails.actionDetails.bindingRoutingKey = this.placeholders[sendDetails.selectedOption.exchangeType];
                }
                this.$set(selectedTab['status'], "isLoading", true);
                this.$ipc.invoke("NewAction", sendDetails).then(state => {
                    if (state['status']) {
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
                    } else {
                        this.$set(selectedTab['status'], "isLoading", false);
                        this.$set(selectedTab['status'], "isActive", false);
                        console.error(state['message'])
                    }
                });
            },
            async terminateAction(id) {
                const self = this;
                return await this.$ipc.invoke("TerminateAction", id).then(state => {
                    if (state['status']) {
                        self.$ipc.removeAllListeners(id);
                        return true;
                    } else {
                        console.error(state['message']);
                        return false;
                    }
                })
            },
            terminateTab(index) {
                const tab = this.$store.getters.getTabs[index];
                if (tab['status']['isActive'] || tab['status']['isLoading']) {
                    this.terminateAction(tab['id']).then(state => {
                        if (state) {
                            this.$store.commit('deleteTab', index)
                        }
                    });
                } else {
                    this.$store.commit('deleteTab', index)
                }
            }
        },
        computed: {
            parsedStatus: {
                get() {
                    const currentTabStatus = this.$store.getters.getCurrentTab.status;
                    const currentTabLimits = this.$store.getters.getCurrentTab.actionDetails;
                    let messagePercent = 0, sizePercent = 0, secondsPercent = 0;
                    if (currentTabLimits['countLimit'].status && Object.prototype.hasOwnProperty.call(currentTabStatus, "Count")) {
                        messagePercent = (currentTabStatus['Count'] / currentTabLimits['countLimit'].value) * 100;
                    }
                    if (currentTabLimits['sizeLimit'].status && Object.prototype.hasOwnProperty.call(currentTabStatus, "SizeKB")) {
                        sizePercent = (currentTabStatus['SizeKB'] / currentTabLimits['sizeLimit'].value) * 100;
                    }
                    if (currentTabLimits['timeLimit'].status && Object.prototype.hasOwnProperty.call(currentTabStatus, "Duration")) {
                        let durationList = currentTabStatus['Duration'].split(":");
                        let secondsPassed = (+durationList[0]) * 60 * 60 + (+durationList[1]) * 60 + (+durationList[2]);
                        secondsPercent = Math.round((secondsPassed / currentTabLimits['timeLimit'].value) * 100);
                    }

                    return [messagePercent, sizePercent, secondsPercent]
                }
            }
        }
    }
</script>

<style scoped>
</style>