module.exports = {
    configureWebpack: {
        target: "electron-renderer",
        devtool: 'source-map'
    },
    pluginOptions: {
        electronBuilder: {
            nodeIntegration: true,
            builderOptions: {
                extraFiles: [
                    {
                        from: "LettuceIO",
                        to: "LettuceIO",
                        filter: ["**/*"]
                    }
                ]
            }
        }
    }
};