<template>
    <div v-if="$store.getters.getCurrentTab !== undefined">
        <Connection/>
        <Settings/>
        <component :is="$store.getters.getCurrentTab.actionType"/>
    </div>
</template>

<script>
    import Connection from "./ActionWindow/Connection";
    import Settings from "./Settings";
    import Record from "./ActionWindow/Actions/Record";
    import Publish from "./ActionWindow/Actions/Publish";
    export default {
        name: "ActionWindow",
        components: {
            Connection,
            Settings,
            Record,
            Publish
        }
    }
</script>

<style scoped>

</style>