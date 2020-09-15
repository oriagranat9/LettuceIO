<template>
    <div class="progress progressStyle" style="height: 8px">
        <div class="progress-bar innerProgressStyle" role="progressbar" v-bind:style="`width:${value}%`" aria-valuemin="0" aria-valuemax="100"
             :aria-valuenow="value"></div>
    </div>
</template>

<script>
    export default {
        name: "progressInput",
        props: ['value']
    }
</script>

<style scoped>
    .progressStyle{
        background-color: var(--bg-tertiary);
    }
    .innerProgressStyle {
        background-color: var(--brand-secondary)
    }
</style>