<template>
    <div class="form-group form-inline" style="white-space: nowrap">
        <label :for="id" class="text-light justify-content-start" style="user-select: none; white-space: pre">{{text}}</label>
        <select :id="id" class="form-control custom-select custom-select-sm text-light lettuce-input"
                @input="$emit('input', $event.target.value)" :value="value">
            <option v-for="option in options" v-bind:key="option">{{option}}</option>
        </select>
    </div>
</template>

<script>
    export default {
        name: "selectInput",
        props: ['text', 'value', 'options'],
        data() {
            return {
                id: null
            }
        },
        mounted() {
            this.id = this.uid;
        }
    }
</script>

<style scoped>
    label {
        font-size: small;
    }
    select {
        width: 100% !important;
    }
</style>