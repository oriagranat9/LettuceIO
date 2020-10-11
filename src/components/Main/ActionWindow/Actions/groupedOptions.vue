<template>
  <div class="main-margins">
    <div v-for="(option, index) in options" v-bind:key="index">

      <div class="row">
        <div class="col-5 justify-content-start align-self-center">
          <div class="form-group custom-control custom-checkbox checkbox-align grouped-row align-self-center"
               style="vertical-align: center;">
            <input v-model="value[option.key]['status']" :id="$id('group' + index)" type="checkbox"
                   @input="$emit('input', value)"
                   class="custom-control-input custom-control-input-green"
                   v-bind:disabled="disabled !== undefined ? disabled : false">
            <label :for="$id('group' + index)"
                   class="custom-control-label text-light checkbox-label align-center">{{ option.text }}</label>
          </div>
        </div>
        <div class="col-7 align-self-center justify-content-start">
          <numberInput v-model="value[option.key]['value']"
                       @input="$emit('input', value)"
                       :text="option.inputText"
                       :disabled="!value[option.key]['status'] || (disabled !== undefined ? disabled : false)"/>
        </div>
      </div>
      <progress-input :id="$id('progress' + index)" :value="progressValues !== undefined ? progressValues[index] : 0"
                      :disabled="!value[option.key]['status']"
                      style="margin-left: 16px"/>
      <b-tooltip :target="$id('progress' + index)" triggers="hover">{{ progressValues[index] }} %</b-tooltip>
    </div>
  </div>
</template>

<script>
import numberInput from "../../../inputs/numberInput";
import progressInput from "../../../inputs/progressInput";

export default {
  name: "groupedOption",
  props: ['options', 'value', 'progressValues', 'disabled'],
  components: {
    numberInput,
    progressInput
  },
  data() {
    return {
      id: null,
    }
  },
  mounted() {
    this.id = this.uid;
  }
}
</script>

<style scoped>
.align-center {
  position: relative;
  transform: translateY(50%);
}

.form-group {
  margin: 0 !important;
}

.grouped-row {
  margin: 0;
}

.main-margins {
  margin: 20px;
}

.checkbox-label {
  user-select: none;
  font-size: small;
  white-space: nowrap
}

.checkbox-align {
  text-align: start;
}
</style>