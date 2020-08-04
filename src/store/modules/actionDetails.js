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
    isRandom: false,
    isLoop: false,
    playback: false,
    routingKeyDetails: {
        isCustom: false,
        customValue: "",
        isRandom: false
    }
};

export {recordDetails, publishDetails}