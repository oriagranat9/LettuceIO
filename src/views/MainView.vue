<template>
    <div class="contents">
        <div class="content">
            <Tabs/>
        </div>
    </div>
</template>

<script>
    import Tabs from "../components/Main/Tabs";
    export default {
        name: "MainView",
        components:{
            Tabs
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