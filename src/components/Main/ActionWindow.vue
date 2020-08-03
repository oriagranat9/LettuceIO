<template>
    <div v-if="$store.getters.getCurrentTab !== undefined">
        <Connection/>
    </div>
</template>

<script>
    import Connection from "./ActionWindow/Connection";

    export default {
        name: "ActionWindow",
        components: {
            Connection
        }
    }
</script>

<style scoped>

</style>