<template>
    <div>
        <grouped-options v-model="actionDetails" :options="recordOptions"/>
    </div>
</template>

<script>
    import groupedOptions from "./groupedOptions";

    export default {
        name: "Record",
        data() {
            return {
                recordOptions: [
                    {
                        text: "message count",
                        inputText: "messages",
                        key: "countLimit"
                    },
                    {
                        text: "total size",
                        inputText: "Mb",
                        key: "sizeLimit"
                    },
                    {
                        text: "record time",
                        inputText: "sec",
                        key: "timeLimit"
                    }
                ],
            }
            // optional when recording from exchange add the routing key option
        },
        components: {
            groupedOptions
        },
        computed: {
            actionDetails: {
                get() {
                    return this.$store.getters.getCurrentTab.actionDetails
                },
                set(value) {
                    this.$store.state.tabs.tabs[this.$store.getters.getTabIndex].actionDetails = value;
                }
            }
        }
    }
</script>

<style scoped>

</style>