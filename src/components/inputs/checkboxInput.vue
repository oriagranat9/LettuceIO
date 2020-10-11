<template>
  <div class="justify-content-start">
    <label style="font-size: small"
           class="text-light align-middle justify-content-start">{{ text }}</label>
    <div class="form-group custom-control custom-checkbox justify-content-start">
      <input v-model="tmp" :id="id" type="checkbox"
             class="custom-control-input-green custom-control-input">
      <label :for="id" class="custom-control-label text-light"/>
    </div>
  </div>
</template>

<script>
export default {
  name: "checkboxInput",
  props: ['text', 'value'],
  data() {
    return {
      id: null
    }
  },
  mounted() {
    this.id = this.uid;
  },
  computed: {
    tmp: {
      get() {
        return this.value;
      },
      set(value) {
        this.$emit('input', value)
      }
    }
  }
}
</script>

<style scoped>
.custom-checkbox {
  display: block !important;
}
</style>