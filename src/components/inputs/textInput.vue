<template>
  <div class="form-group form-inline ">
    <label :for="id" class="text-light justify-content-start"
           style="user-select: none; white-space: nowrap">{{ text }}</label>
    <input :id="id" :value="value"
           v-bind:placeholder="placeholder !== undefined ? placeholder : ''"
           @focus="e => $emit('focus', e)"
           @blur="e => $emit('blur', e)"
           v-on:input="updateValue($event.target.value)"
           class="form-control form-control-sm lettuce-input text-light " type="text" :disabled="disabled">
  </div>
</template>

<script>
export default {
  name: "textInput",
  props: ['text', 'value', 'disabled', 'placeholder'],
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