<template>
    <div style="position: relative">
        <div id="publish" class="row justify-content-start">
            <div class="col-4">
                <grouped-options :progress-values="progressValues" v-model="actionDetails" :options="publishOptions"/>
            </div>
            <div class="col-8 justify-content-start" style="margin-top: 20px">
                <div style="text-align: start; margin: 20px" class="top">
                    <div class="form-group custom-control custom-checkbox checkbox-align grouped-row align-self-center">
                        <input v-model="actionDetails.isShuffle" :id="$id('random')" type="checkbox"
                               class="custom-control-input custom-control-input-green">
                        <label :for="$id('random')"
                               class="custom-control-label text-light checkbox-label"
                               style="font-size: small">Shuffle Order</label>
                    </div>

                    <div class="form-group custom-control custom-checkbox checkbox-align grouped-row align-self-center">
                        <input v-model="actionDetails.isLoop" :id="$id('loop')" type="checkbox"
                               class="custom-control-input custom-control-input-green">
                        <label :for="$id('loop')"
                               class="custom-control-label text-light checkbox-label"
                               style="font-size: small">Loop</label>
                    </div>

                    <div class="form-group custom-control custom-checkbox checkbox-align grouped-row align-self-center top">
                        <input v-model="actionDetails.playback" :id="$id('playback')" type="checkbox"
                               class="custom-control-input custom-control-input-green">
                        <label :for="$id('playback')"
                               class="custom-control-label text-light checkbox-label"
                               style="font-size: small">Playback</label>
                    </div>

                    <div class="row">
                        <div class="col-3 justify-content-start">
                            <div class="form-group custom-control custom-checkbox checkbox-align grouped-row">
                                <input v-model="actionDetails.routingKeyDetails.isCustom" :id="$id('routing')"
                                       type="checkbox"
                                       class="custom-control-input custom-control-input-green">
                                <label :for="$id('routing')"
                                       class="custom-control-label text-light checkbox-label" style="font-size: small">Custom
                                    Routing key</label>
                            </div>
                        </div>
                        <div class="col-4 align-self-center justify-content-start" style="text-align: start"
                             v-if="actionDetails.routingKeyDetails.isCustom">
                            <text-input v-model="actionDetails.routingKeyDetails.customValue"
                                        :disabled="actionDetails.routingKeyDetails.isRandom"/>
                            <div class="form-group custom-control custom-checkbox checkbox-align grouped-row align-self-baseline">
                                <input v-model="actionDetails.routingKeyDetails.isRandom" :id="$id('custom')"
                                       type="checkbox"
                                       class="custom-control-input custom-control-input-green">
                                <label :for="$id('custom')"
                                       class="custom-control-label text-light checkbox-label" style="font-size: small">Random
                                    Key</label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</template>

<script>
    import groupedOptions from "./groupedOptions";
    import TextInput from "../../../inputs/textInput";

    export default {
        name: "Publish",
        props: ['progressValues'],
        data() {
            return {
                publishOptions: [
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
                    },
                ]
            }
        },
        components: {
            TextInput,
            groupedOptions,
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
        //rate
        // ordered or not
        // loop, playback(time delta)
        // custom routing routing key (or random)
    }
</script>

<style scoped>

    .top {
        margin-top: 16px;
    }

    #publish {
        margin: 0;
    }

    label {
        user-select: none;
    }
</style>