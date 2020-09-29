<template>
    <div class="form-group form-inline" style="white-space: nowrap; font-size: small;">
        <label :for="id" class="text-light justify-content-start"
               style="user-select: none; white-space: pre">{{text}}</label>
        <v-select :options="options" :id="id" class="select-wrapper" @input="$emit('input', $event)" :value="value"/>
    </div>
</template>

<script>
    import VSelect from '@alfsnd/vue-bootstrap-select'

    export default {
        name: "selectInput",
        props: ['text', 'value', 'options'],
        components: {
            VSelect
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

<style>
    .select-wrapper {
        width: 100% !important;
        height: 32px !important;
    }

    /*Scroll bar css style for the select overflow*/
    /* width */
    .select-wrapper ::-webkit-scrollbar {
        width: 5px;
    }

    /* Track */
    .select-wrapper ::-webkit-scrollbar-track {
        background: var(--bg-tertiary);
    }

    /* Handle */

    .select-wrapper ::-webkit-scrollbar-thumb {
        background: var(--brand-primary);
        border-radius: 5px;
    }

    /*override over the default V select css */
    .select-wrapper .v-select-toggle {
        background-color: var(--bg-secondary) !important;
        border-color: var(--brand-primary) !important;
        color: #f8f9fa !important;
    }

    .select-wrapper .v-select-toggle:hover {
        background-color: var(--bg-secondary) !important;
        border-color: var(--brand-primary) !important;
        color: #f8f9fa !important;
    }

    .select-wrapper .v-select-toggle:focus {
        background-color: var(--bg-secondary) !important;
        border-color: var(--brand-primary) !important;
        color: #f8f9fa !important;
        outline-color: var(--brand-secondary) !important;;
    }

    .select-wrapper .v-dropdown-container {
        max-height: 40vmin;
        overflow-y: auto;
        overflow-x: hidden;
        background-color: var(--bg-secondary) !important;
    }

    .select-wrapper .v-dropdown-item {
        text-overflow: ellipsis;
        overflow-x: hidden;
        background-color: var(--bg-secondary) !important;
        color: #f8f9fa !important;
    }

    .select-wrapper .v-dropdown-item:hover {
        background-color: var(--brand-primary) !important;
    }

</style>