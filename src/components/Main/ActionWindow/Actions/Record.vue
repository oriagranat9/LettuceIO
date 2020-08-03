<template>
    <div>
        <grouped-options :options="recordOptions"/>
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
                        disabled: false,
                        text: "bitch",
                        visible: true,
                        input: true,
                        inputDisabled: false,
                        inputText: "hi"
                    }
                ]
            }
        },
        components: {
            groupedOptions
        }
    }
</script>

<style scoped>

</style>