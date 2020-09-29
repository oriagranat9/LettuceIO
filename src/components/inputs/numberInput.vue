<template>
    <div class="form-group form-inline">
        <label :for="id" class="text-light justify-content-start" style="user-select: none">{{text}}</label>
        <input :id="id" :value="value"
               v-bind:max="max === undefined ? '' : max"
               v-bind:min="min === undefined ? '' : min"
               @focus="e => $emit('focus', e)"
               @blur="onBlur"
               v-on:input="updateValue($event.target.value)"
               class="form-control form-control-sm lettuce-input text-light " type="number" :disabled="disabled">
    </div>
</template>

<script>
    export default {
        name: "numberInput",
        props: ['text', 'value', 'disabled', 'max', 'min'],
        methods: {
            updateValue(value) {
                this.$emit('input', value)
            },
            onBlur(event) {
                let min = this.min === undefined ? Number.MIN_SAFE_INTEGER : this.min;
                let max = this.max === undefined ? Number.MAX_SAFE_INTEGER : this.max;
                let value = Math.min(Math.max(parseInt(this.value), min), max);
                if (value !== this.value) {
                    this.updateValue(value)
                }
                this.$emit('blur', event)
            }
        },
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
    input {
        width: 100% !important;
    }

    label {
        font-size: small;
        width: auto;
    }

    .text-input {
        white-space: nowrap;
    }
</style>