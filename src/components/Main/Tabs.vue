<template>
    <div>
        <div class="tabWrapper">
            <ul class="nav nav-tabs text-center nav-bar">
                <draggableComponent style="display: flex" v-model="tabs" @end="dragCallBack">
                    <li class="nav-item" v-for="(tab, index) in tabs" v-bind:key="index">
                        <a class="nav-link tab text-center text-light"
                           @click="changeTab(index)"
                           @mousedown.middle="e=> {e.stopPropagation(); deleteTab(index);}"
                           v-bind:class="index === $store.getters.getTabIndex ? 'activeTab': ''"
                        >
                            <div class="tabName">
                                {{tab.name}}
                            </div>
                            <b-icon scale="1.2" icon="x" class="tabIcon"
                                    @click="e => {e.stopPropagation(); deleteTab(index)}"/>
                        </a>
                    </li>
                </draggableComponent>
                <li class="nav-item">
                    <a class="nav-link tab text-center text-light" @click="createTab">
                        <b-icon scale="1.2" icon="plus" class="newTab"/>
                    </a>
                </li>
            </ul>
        </div>
    </div>
</template>

<script>
    import draggableComponent from 'vuedraggable'

    export default {
        name: "Tabs",
        components: {
            draggableComponent
        },
        computed: {
            tabs: {
                get() {
                    return this.$store.getters.getTabs;
                },
                set(value) {
                    this.$store.commit('setTabs', value)
                }
            }
        },
        methods: {
            changeTab(index) {
                this.$store.commit('changeTabIndex', index)
            },
            deleteTab(index) {
                this.$emit("delete", index)
            },
            createTab() {
                this.$store.commit('createTab')
            },
            dragCallBack(event) {
                const oldIndex = event['oldIndex'];
                const newIndex = event['newIndex'];
                if (this.$store.getters.getTabIndex === oldIndex) {
                    this.changeTab(newIndex);
                } else {
                    if (oldIndex < this.$store.getters.getTabIndex && newIndex >= this.$store.getters.getTabIndex) {
                        this.changeTab(this.$store.getters.getTabIndex - 1);
                    } else if (oldIndex > this.$store.getters.getTabIndex && newIndex <= this.$store.getters.getTabIndex) {
                        this.changeTab(this.$store.getters.getTabIndex + 1)
                    }
                }

            }
        },
        created() {
            if (this.tabs.length <= 0) {
                this.createTab();
            }
        }
    }
</script>

<style scoped>
    .tabWrapper {
        padding-top: 5px;
        background-color: var(--bg-primary);
    }

    .nav-bar {
        border-bottom: 1px solid var(--hairline-reguar);
    }

    .tab {
        user-select: none;
        cursor: pointer;
        display: flex;
        border: 1px solid var(--hairline-reguar) !important;
        margin-left: 3px;
    }

    .tabName {
        line-height: 20px;
        overflow: hidden;
        text-overflow: clip;
        white-space: nowrap;
        vert-align: middle;
        vertical-align: middle;
        max-width: 200px;
        color: ghostwhite !important;
    }

    .tab:hover .tabName {
        transition: color .2s ease;
        color: white !important;
    }

    .tabIcon {
        color: ghostwhite !important;
    }

    .tabIcon:hover {
        color: red !important;
        transition: color .2s ease;
    }

    .activeTab {
        background-color: var(--bg-secondary);
        border-top: 2px solid var(--brand-primary) !important;
        padding-top: 7px;
        border-bottom-color: var(--bg-secondary);
    }

    .activeTab .tabName {
        color: white !important;
    }

    .newTab:hover {
        transition: color .2s ease;
        color: var(--brand-secondary);
    }

</style>