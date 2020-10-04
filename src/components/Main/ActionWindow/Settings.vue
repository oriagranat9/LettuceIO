<template>
  <div class="margins">
    <div class="row span justify-content-start">
      <div class="col-2 align-self-end" style="text-align: start">
        <button @click="selectFolder" style="margin-top: 5px"
                class="btn btn-sm btn-outline-success align-self-center align-middle">Select
          Folder
        </button>
      </div>
      <div class="col-5 align-self-center" style="max-width: 300px; text-align: start">
        <select-input v-model="selectedOption" :options="optionList" :text="selectName" style="margin: 0"/>
        <span style="font-size: small; position: absolute" class="text-light"
              v-if="$store.getters.getCurrentTab.selectedOption.type === 'Exchange'">
                    Type: {{ $store.getters.getCurrentTab.selectedOption.exchangeType }}
                </span>
      </div>
      <div class="col-3" style="position: relative; margin-right: 5px"
           v-if="$store.getters.getCurrentTab.actionType === 'Publish'">
        <div class="publishClass">
          <number-input v-model="actionDetails.rateDetails.rate" :min="0" text="Message Rate (Hz)"/>
          <number-input v-model="actionDetails.rateDetails.multiplier" :min="1" :max="4"
                        text="Thread Multiplier"/>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import SelectInput from "../../inputs/selectInput";
import NumberInput from '../../inputs/numberInput'

export default {
  name: "Settings",
  components: {
    SelectInput,
    NumberInput
  },
  data() {
    return {}
  },
  methods: {
    selectFolder() {
      const {dialog} = require('electron').remote;
      dialog.showOpenDialog({properties: ['openDirectory', 'createDirectory']}).then(value => {
        this.selectedFolder = value.filePaths[0];
      })
    }

  },
  computed: {
    actionDetails: {
      get() {
        return this.$store.getters.getCurrentTab.actionDetails;
      },
      set(value) {
        this.$store.commit('setTabValue', {key: "actionDetails", value: value});
      }
    },
    optionList: {
      get() {
        let tmp = [];
        for (let i of this.$store.getters.getCurrentTab.tmpLists.optionList) {
          tmp.push(`${i['type']}/${i['name']}`)
        }
        return tmp;
      }
    },
    selectName: {
      get() {
        if (this.$store.getters.getCurrentTab.actionType === "Publish") {
          return "To";
        } else {
          return "From"
        }
      }
    },
    selectedFolder: {
      get() {
        return this.$store.getters.getCurrentTab.folderPath;
      },
      set(value) {
        this.$store.commit('setTabValue', {key: "folderPath", value: value});
      }
    },
    selectedOption: {
      get() {
        let selected = this.$store.getters.getCurrentTab.selectedOption;
        if (selected.name !== "" && selected.type !== "") {
          return `${selected['type']}/${selected['name']}`
        } else {
          return ""
        }
      },
      set(value) {
        let index = this.optionList.indexOf(value);
        let selectedOption = this.$store.getters.getCurrentTab.tmpLists.optionList[index];
        this.$store.commit('setTabValue', {key: "selectedOption", value: selectedOption});
      }
    },
  }
}
</script>

<style scoped>
.publishClass {
  position: absolute;
  z-index: 4;
  right: 0;
}

.publishClass div {
  margin-top: 5px;
  margin-bottom: 0;
}

.margins {
  margin: 20px;
}

.span {
  width: 100% !important;
}

</style>