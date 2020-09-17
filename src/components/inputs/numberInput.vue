<template>
    <div class="form-group form-inline">
        <label :for="id" class="text-light justify-content-start" style="user-select: none">{{text}}</label>
        <input :id="id" :value="value"
               @focus="e => $emit('focus', e)"
               @blur="e => $emit('blur', e)"
               min="0"
               v-on:input="updateValue($event.target.value)"
               class="form-control form-control-sm lettuce-input text-light " type="number" :disabled="disabled">
    </div>
</template>

<script>
    export default {
        name: "numberInput",
        props: ['text', 'value', 'disabled'],
        methods: {
            updateValue(value) {
                this.$emit('input', value)
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