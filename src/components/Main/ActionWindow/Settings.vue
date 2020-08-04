<template>
    <div class="margins">
        <div class="row span justify-content-start">
            <div class="col-3 align-self-end" style="text-align: start">
                <button @click="selectFolder" style="margin-top: 5px" class="btn btn-outline-success align-self-center align-middle">Select
                    Folder
                </button>
            </div>
            <div class="col-5 align-self-center" style="max-width: 200px; text-align: start">
                <select-input v-model="selectedOption" :options="optionList" :text="selectName" style="margin: 0"/>
            </div>
        </div>
    </div>
</template>

<script>
    import SelectInput from "../../inputs/selectInput";

    export default {
        name: "Settings",
        components: {SelectInput},
        data() {
            return {
                selectedOption: "",
                optionList: ["brrrrrrr"]
            }
        },
        methods: {
            selectFolder() {
                const {dialog} = require('electron').remote;
                dialog.showOpenDialog({properties: ['openDirectory']}).then(

                )
            }

        },
        computed: {
            selectName: {
                get() {
                    if (this.$store.getters.getCurrentTab.actionType === "Publish") {
                        return "To";
                    } else {
                        return "From"
                    }
                }
            }
        }
    }
</script>

<style scoped>
    .margins {
        margin: 20px;
    }

    .span {
        width: 100% !important;
    }

</style>