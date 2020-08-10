module.exports = {
    configureWebpack: {
        target: "electron-renderer"
    },
    pluginOptions: {
        electronBuilder: {
            nodeIntegration: true
        }
    }
};