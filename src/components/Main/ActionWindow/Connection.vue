<template>
    <div class="connectionClass">
        <div class="form-inline form-margins">
            <div class="form-group span">
                <div class="row span justify-content-start">
                    <div class="col-lg-2 " style="margin-top: 5px">
                        <select-input v-model="actionType" text="Action Type" :options="options"/>
                        <checkbox-input text="Advanced" v-model="showAdvanced"/>
                    </div>
                    <div class="col-lg-6">
                        <div class="row">
                            <div class="col-lg-10" style="margin-top: 5px">
                                <text-input v-model="connectionString" text="Connection String"
                                            @blur="()=> {parseConnectionString(); queryAllVhosts()}"
                                            @focus="constructConnectionString"/>
                            </div>
                            <div class="col-md-2">
                                <text-input @blur="queryAllVhosts" class="space" v-model="connectionDetails.apiPort"
                                            text="API Port"/>
                            </div>
                        </div>
                        <div class="row" v-if="showAdvanced">
                            <div class="col-lg-10" style="margin-top: 5px">
                                <text-input v-model="connectionDetails.amqpHostName" text="AMQP Hostname"/>
                            </div>
                            <div class="col-md-2">
                                <text-input class="space" v-model="connectionDetails.amqpPort" text="AMQP Port"/>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-1">
                        <text-input @blur="queryAllVhosts" v-model="connectionDetails.username" class="space"
                                    text="Username"/>
                        <text-input @blur="queryAllVhosts" v-model="connectionDetails.password" class="space"
                                    text="Password"/>
                        <select-input @input="queryAllOptions" v-model="connectionDetails.vhost" :options="queriedList.vhosts"
                                      class="space"
                                      text="VHost"/>
                    </div>
                    <div class="col-lg-2 d-flex justify-content-center">
                        <button style="width: 50%"
                                class="btn btn-dark lettuce-button align-self-center">Start
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
    import {queryVHosts, queryOptions} from './rabbitQuery'

    export default {
        name: "Connection",
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
            }
        },
        methods: {
            queryAllVhosts() {
                queryVHosts(this.connectionDetails.apiHostName,
                    this.connectionDetails.apiPort,
                    this.connectionDetails.username,
                    this.connectionDetails.password).then(value => {
                        this.queriedList.vhosts = value;
                    }
                )
            },
            queryAllOptions() {
                queryOptions(this.connectionDetails.apiHostName,
                    this.connectionDetails.apiPort,
                    this.connectionDetails.username,
                    this.connectionDetails.password,
                    this.connectionDetails.vhost).then(value => {
                    this.queriedList.optionList = value;
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
                    if (this.connectionDetails.apiPort !== "") {
                        this.connectionString += `:${this.connectionDetails.apiPort}`
                    }
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
        computed: {
            queriedList: {
                get() {
                    return this.$store.getters.getCurrentTab.tmpLists;
                },
                set(value) {
                    this.$store.commit('setTabValue', {name: "tmpLists", value: value})
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
                    this.$store.commit('setTabValue', {key: "actionType", value: value});
                    this.$store.commit("changeActionDetails")
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