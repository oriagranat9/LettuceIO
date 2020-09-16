<template>
    <div>
        <Tabs @delete="terminateTab"/>
        <div v-if="$store.getters.getCurrentTab !== undefined">
            <Connection @action="Action"/>
            <Settings/>
            <component id="action" :is="$store.getters.getCurrentTab.actionType"/>
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
                        return false
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
        }
    }
</script>

<style scoped>
</style>