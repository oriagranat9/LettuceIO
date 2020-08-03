<template>
    <div>
        <div class="row" v-for="(option, index) in options" v-bind:key="index">
            <div class="col-1">
                <div class="form-group custom-control custom-checkbox">
                    <input :id="id" type="checkbox" class="custom-control-input custom-control-input-green">
                    <label style="user-select: none" :for="id" class="custom-control-label text-light ">{{option.text}}</label>
                </div>
            </div>
            <div class="col-2">
                <textInput :text="options.inputText"/>
            </div>
        </div>
    </div>
</template>

<script>
    import textInput from "../../../inputs/textInput";

    export default {
        name: "groupedOption",
        props: ['options'],
        components: {
            textInput
        },
        data() {
            return {
                id: null,
            }
        },
        mounted() {
            this.id = this._uid;
        }
    }
</script>

<style scoped>

</style>