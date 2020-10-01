module.exports = {
    configureWebpack: {
        target: "electron-renderer",
        devtool: 'source-map'
    },
    pluginOptions: {
        electronBuilder: {
            nodeIntegration: true,
            builderOptions: {
                build:{
                    productName: "LettuceIO",
                    uninstallDisplayName: "LettuceIO",
                    artifactName: "LettuceIO.${ext}",
                },
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