<template>
    <div class="connectionClass">
        <div class="form-inline form-margins">
            <div class="form-group span">
                <div class="row span justify-content-start">
                    <div class="col-lg-2 " style="margin-top: 5px">
                        <select-input :disabled="disabled" v-model="actionType" text="Action Type" :options="options"/>
                        <checkbox-input text="Advanced" v-model="showAdvanced"/>
                    </div>
                    <div class="col-lg-6">
                        <div class="row">
                            <div class="col-lg-9" style="margin-top: 5px">
                                <text-input v-model="connectionString" text="Connection String"
                                            :disabled="disabled"
                                            @blur="()=> {parseConnectionString(); queryAllVhosts(); queryAmqp()}"
                                            @focus="constructConnectionString"/>
                            </div>
                            <div class="col-md-3">
                                <text-input @blur="()=> {queryAllVhosts(); queryAmqp()}" class="space"
                                            :disabled="disabled"
                                            v-model="connectionDetails.apiPort"
                                            text="API Port"/>
                            </div>
                        </div>
                        <div class="row" v-if="showAdvanced">
                            <div class="col-lg-9" style="margin-top: 5px">
                                <text-input v-model="connectionDetails.amqpHostName" text="AMQP Hostname"
                                            :disabled="disabled"/>
                            </div>
                            <div class="col-md-3">
                                <text-input class="space" v-model="connectionDetails.amqpPort" text="AMQP Port"
                                            :disabled="disabled"/>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-2">
                        <text-input @blur="()=> {queryAllVhosts(); queryAmqp()}" v-model="connectionDetails.username"
                                    class="space"
                                    text="Username" :disabled="disabled"/>
                        <text-input @blur="()=> {queryAllVhosts(); queryAmqp()}" v-model="connectionDetails.password"
                                    class="space"
                                    text="Password" :disabled="disabled"/>
                        <select-input @input="()=> {queryAllVhosts(); queryAmqp()}" v-model="connectionDetails.vhost"
                                      :options="queriedList.vhosts"
                                      :disabled="disabled"
                                      class="space"
                                      text="VHost"/>
                    </div>
                    <div class="col-lg-2 d-flex justify-content-center">
                        <button style="width: 90%" @click="$emit('action')" v-bind:disabled="!isValid"
                                class="btn btn-dark lettuce-button align-self-center">
                            <span v-if="$store.getters.getCurrentTab.status['isLoading']"
                                  class="spinner-border spinner-border-sm" role="status" aria-hidden="true"/>
                            <span v-if="$store.getters.getCurrentTab.status['isLoading']" class="sr-only"/>
                            {{
                            $store.getters.getCurrentTab.status['isLoading'] ? "Loading..." :
                            ($store.getters.getCurrentTab.status['isActive'] ? "stop" : "start")
                            }}
                        </button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    import textInput from "../../inputs/textInput";
    import selectInput from "../../inputs/selectInput";
    import checkboxInput from "../../inputs/checkboxInput";

    export default {
        name: "Connection",
        props: ['disabled'],
        components: {
            textInput,
            selectInput,
            checkboxInput
        },
        data() {
            return {
                options: [
                    "Record",
                    "Publish"
                ],
                connectionString: "",
                showAdvanced: false,
                tmpHostName: String
            }
        },
        methods: {
            queryAmqp() {
                this.$ipc.invoke("queryPort", {
                    hostname: this.connectionDetails.apiHostName,
                    port: this.connectionDetails.apiPort,
                    username: this.connectionDetails.username,
                    password: this.connectionDetails.password
                }).then(value => {
                    if (value !== undefined && (this.connectionDetails.amqpHostName === "" || this.tmpHostName === this.connectionDetails.amqpHostName)) {
                        this.connectionDetails.amqpHostName = value["hostname"];
                        this.connectionDetails.amqpPort = value["port"];
                    }
                }).catch(() => {
                    this.connectionDetails.amqpHostName = "";
                    this.connectionDetails.amqpPort = "";
                    this.$toast.error(`could not connect to rabbit at ${this.connectionDetails.apiHostName}:${this.connectionDetails.apiPort}`, {
                      timeout: 2000,
                    });
                });
                this.tmpHostName = this.connectionDetails.apiHostName;
            },
            queryAllVhosts() {
                this.$ipc.invoke("queryVhost", {
                    hostname: this.connectionDetails.apiHostName,
                    port: this.connectionDetails.apiPort,
                    username: this.connectionDetails.username,
                    password: this.connectionDetails.password
                }).then(value => {
                    if (value.length > 0) {
                        this.queriedList.vhosts = value;
                        if (this.connectionDetails.vhost !== "") {
                            this.queryAllOptions();
                        }
                    }
                });
            },
            queryAllOptions() {
                this.$ipc.invoke("queryOptions", {
                    hostname: this.connectionDetails.apiHostName,
                    port: this.connectionDetails.apiPort,
                    username: this.connectionDetails.username,
                    password: this.connectionDetails.password,
                    vhost: this.connectionDetails.vhost
                }).then(value => {
                    if (value.length > 0) {
                        this.queriedList.optionList = value;
                        this.$store.commit('setTabValue', {
                            key: "selectedOption",
                            value: this.queriedList.optionList[0]
                        });
                    }
                })
            },
            parseConnectionString() {
                let conn = this.connectionString.replace("http://", "").replace("amqp://", "")
                    .split("/")[0];
                let tmp = conn.split("@");
                let uri = tmp[0];
                if (tmp.length > 1) {
                    let authList = tmp[0].split(":");
                    this.connectionDetails.username = authList[0];
                    this.connectionDetails.password = authList[1];
                    uri = tmp[1]
                }
                let uriList = uri.split(":");
                this.connectionDetails.apiHostName = uriList[0];
                if (uriList.length > 1) {
                    this.connectionDetails.apiPort = uriList[1];
                    this.connectionDetails.apiHostName = uriList[0];
                }
                this.minimizeConnectionString();
            },
            minimizeConnectionString() {
                if (this.connectionDetails.apiHostName !== "") {
                    this.connectionString = this.connectionDetails.apiHostName;
                }
            },
            constructConnectionString() {
                if (this.connectionDetails.apiHostName !== "") {
                    this.connectionString = "http://";
                    if (this.connectionDetails.username !== "" && this.connectionDetails.password !== "") {
                        this.connectionString += `${this.connectionDetails.username}:${this.connectionDetails.password}@`
                    }
                    this.connectionString += this.connectionDetails.apiHostName;
                    if (this.connectionDetails.apiPort !== "") {
                        this.connectionString += `:${this.connectionDetails.apiPort}`
                    }
                }
            }
        },
        watch: {
            connectionDetails: {
                handler() {
                    this.connectionString = "";
                    this.minimizeConnectionString()
                }
            }
        },
        computed: {
            queriedList: {
                get() {
                    return this.$store.getters.getCurrentTab.tmpLists;
                },
                set(value) {
                    this.$store.commit('setTabValue', {key: "tmpLists", value: value})
                }
            },
            connectionDetails: {
                get() {
                    return this.$store.getters.getCurrentTab.connection;
                },
            },
            actionType: {
                get() {
                    return this.$store.getters.getCurrentTab['actionType'];
                },
                set(value) {
                    // this.$store.commit('setTabValue', {key: "actionType", value: value});
                    this.$store.commit("changeActionDetails", value)
                }
            },
            isValid: {
                get() {
                    const tab = this.$store.getters.getCurrentTab;
                    let isUserValid = tab.connection.username !== "" && tab.connection.password;
                    let isUriValid = tab.connection.amqpHostName !== "" && tab.connection.amqpPort !== "";
                    let isConnectionValid = isUserValid && isUriValid && tab.connection.vhost !== "";
                    let isOptionValid = tab.selectedOption.name !== "" && tab.selectedOption.type !== "";
                    let hasFolderPath = tab.folderPath !== undefined;
                    return isConnectionValid && isOptionValid && hasFolderPath;
                }
            }
        }
    }
</script>

<style scoped>
    .connectionClass {
        width: 100%;
        border-bottom: 1px solid grey;
    }

    .form-margins {
        margin: 20px;
    }

    .span {
        width: 100% !important;
    }

    .space {
        margin-top: 5px;
    }
</style>