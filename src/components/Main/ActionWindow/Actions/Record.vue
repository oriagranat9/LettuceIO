<template>
  <div>
    <div id="record" class="row" style="">
      <div class="col-4">
        <grouped-options :disabled="disabled" :progress-values="progressValues" v-model="actionDetails"
                         :options="recordOptions"/>
      </div>
      <div class="col-3">
        <div style="text-align: start; margin: 20px">
          <text-input v-model="actionDetails.bindingRoutingKey"
                      v-if="$store.getters.getCurrentTab.selectedOption.type === 'Exchange'"
                      text="Binding routing key"
                      :placeholder="placeHolders[$store.getters.getCurrentTab.selectedOption.exchangeType]"
                      :disabled="disabled"/>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import groupedOptions from "./groupedOptions";
import TextInput from "../../../inputs/textInput";
import ExchangePlaceholders from "../ExchangePlaceholders";

export default {
  name: "Record",
  props: ['progressValues', 'disabled'],
  data() {
    return {
      placeHolders: ExchangePlaceholders,
      recordOptions: [
        {
          text: "Message count",
          inputText: "messages",
          key: "countLimit"
        },
        {
          text: "Total size",
          inputText: "kb",
          key: "sizeLimit"
        },
        {
          text: "Duration",
          inputText: "sec",
          key: "timeLimit"
        }
      ],
    }
    // optional when recording from exchange add the routing key option
  },
  components: {
    TextInput,
    groupedOptions
  },
  computed: {
    actionDetails: {
      get() {
        return this.$store.getters.getCurrentTab.actionDetails
      },
      set(value) {
        this.$store.state.tabs.tabs[this.$store.getters.getTabIndex].actionDetails = value;
      }
    }
  }
}
</script>

<style scoped>
#record {
  margin: 0;
}
</style>