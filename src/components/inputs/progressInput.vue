<template>
    <div class="progress progressStyle"
         v-bind:style="disabled ? 'background-color: var(--bg-primary);' : 'background-color: var(--bg-tertiary);'"
         style="height: 4px">
        <div class="progress-bar innerProgressStyle" role="progressbar" v-bind:style="`width:${disabled ? 0 : value}%`"
             aria-valuemin="0" aria-valuemax="100"
             :aria-valuenow="disabled ? 0 : value"></div>
    </div>
</template>

<script>
    export default {
        name: "progressInput",
        props: ['value', 'disabled']
    }
</script>

<style scoped>
    .progressStyle {
        margin-top: 8px;
    }

    .innerProgressStyle {
        background-color: var(--brand-secondary)
    }
</style>