const recordDetails = {
    countLimit: {
        status: false,
        value: 0
    },
    sizeLimit: {
        status: false,
        value: 0
    },
    timeLimit: {
        status: false,
        value: 0
    },
    bindingRoutingKey: ""
};

const publishDetails = {
    countLimit: {
        status: false,
        value: 0
    },
    sizeLimit: {
        status: false,
        value: 0
    },
    timeLimit: {
        status: false,
        value: 0
    },
    isShuffle: false,
    isLoop: false,
    playback: false,
    rateDetails: {
        rate: 10,
        multiplier: 1
    },
    routingKeyDetails: {
        isCustom: false,
        customValue: "",
        isRandom: false
    }
};

export {recordDetails, publishDetails}