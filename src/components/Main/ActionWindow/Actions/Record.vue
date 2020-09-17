<template>
    <div>
        <div id="record" class="row" style="">
            <div class="col-4">
                <grouped-options :progress-values="progressValues" v-model="actionDetails" :options="recordOptions"/>
            </div>
        </div>
    </div>
</template>

<script>
    import groupedOptions from "./groupedOptions";

    export default {
        name: "Record",
        props: ['progressValues'],
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
                        inputText: "kb",
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
    #record {
        margin: 0;
    }
</style>