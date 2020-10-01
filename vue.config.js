module.exports = {
    configureWebpack: {
        target: "electron-renderer",
        devtool: 'source-map'
    },
    pluginOptions: {
        electronBuilder: {
            nodeIntegration: true,
            builderOptions: {
                productName: "Lettuce.IO",
                win: {
                    target: ["zip"],
                    artifactName: "Lettuce.IO.v${version}.${ext}",
                },
                publish: ['github'],
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