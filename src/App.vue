<template>
    <div id="app" class="fullPage">
        <main-view class="fullPage"/>
    </div>
</template>

<script>
    import MainView from "./views/MainView";

    export default {
        name: 'App',
        components: {
            MainView
        },
        methods: {}
    }
</script>

<style>
    #app {
        font-family: Avenir, Helvetica, Arial, sans-serif;
        -webkit-font-smoothing: antialiased;
        -moz-osx-font-smoothing: grayscale;
        text-align: center;
        background: var(--bg-primary);
        position: absolute;
    }

    .fullPage {
        height: 100%;
        width: 100%;
    }

    :root {
        --bg-primary: #282828;
        --bg-secondary: #303030;
        --hairline-reguar: #434343;
        --brand-primary: #04b013;
        --brand-secondary: #07b01d;
        --border-color: #01b026;
        --border-secondary-color: #0bb008;
        --shadow-color: rgba(0, 227, 42, 0.25)

    }
</style>
