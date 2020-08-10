import vue from 'vue'


async function queryVHosts(hostname, port, username, password) {
    let tmp = [];
    if (hostname !== "" && port !== "" && username !== "" && password !== "") {
        let response = await vue.$http.get(getConnString(hostname, port, username, password) + "/api/vhosts", {
            headers: {
                "Content-Type": "application/json"
            }
        });
        if (response.status === 200) {
            for (let vhost of response) {
                tmp.push(vhost['name'])
            }
        }
    }
    return tmp;
}

async function queryOptions(hostname, port, username, password, vhost) {
    let tmp = [];
    if (hostname !== "" && port !== "" && username !== "" && password !== "" && vhost !== "") {
        let queueRequest = vue.$http.get(getConnString(hostname, port, username, password) + `/api/queues/${vhost}`);
        let exchangeRequest = vue.$http.get(getConnString(hostname, port, username, password) + `/api/exchanges/${vhost}`);
        let responses = await Promise.all([queueRequest, exchangeRequest]);

        let queueResponse = responses[0];
        let exchangeResponse = responses[1];

        if (queueResponse.status === 200) {
            for (let queue of queueResponse.data) {
                tmp.push({
                    type: "Queue",
                    name: queue['name']
                })
            }
        }
        if (exchangeResponse.status === 200) {
            for (let exchange of exchangeResponse.data) {
                tmp.push({
                    type: "Exchange",
                    name: exchange['name']
                })
            }
        }
    }
    return tmp;
}

function getConnString(hostname, port, username, password) {
    return `http://${username}:${password}@${hostname}:${port}`
}

export {queryVHosts, queryOptions}