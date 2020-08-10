<template>
    <div class="contents">
        <div class="content">
            <Tabs/>
            <ActionWindow/>
        </div>
    </div>
</template>

<script>
    import Tabs from "../components/Main/Tabs";
    import ActionWindow from "../components/Main/ActionWindow";
    export default {
        name: "MainView",
        components:{
            Tabs,
            ActionWindow
        }
    }
</script>

<style scoped>

    .contents {
        flex-direction: row;
        display: flex;
    }

    .content {
        flex: 1;
    }
</style>