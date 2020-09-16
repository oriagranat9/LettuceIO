<template>
    <div class="contents">
        <div class="content">
            <ActionWindow/>
        </div>
    </div>
</template>

<script>
    import ActionWindow from "../components/Main/ActionWindow";
    export default {
        name: "MainView",
        components:{
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