import axios from 'axios'

async function queryPort({hostname, port, username, password}) {
    if (hostname !== "" && port !== "" && username !== "" && password !== "") {
        let response = await axios.get(getConnString(hostname, port) + "/api/overview", {
            auth: {
                username: username,
                password: password
            },
            headers: {
                "Content-Type": "application/json"
            }
        });
        if (response.status === 200) {
            for (let node of response.data["listeners"]) {
                if (node["protocol"] === "amqp" && node["ip_address"] === "0.0.0.0") {
                    // try's to get amqp port from the amqp listener
                    return {
                        hostname: hostname,
                        port: node["port"]
                    }
                }
            }
            return {
                // if no listener set the hostname and leave the port empty as we cannot discover it
                hostname: hostname,
                port: ""
            }
        }
    }
}

async function queryVHosts({hostname, port, username, password}) {
    let tmp = [];
    if (hostname !== "" && port !== "" && username !== "" && password !== "") {
        let response = await axios.get(getConnString(hostname, port) + "/api/vhosts", {
            auth: {
                "username": username,
                "password": password
            },
            headers: {
                "Content-Type": "application/json",
            }
        });
        if (response.status === 200) {
            for (let vhost of response.data) {
                tmp.push(vhost['name'])
            }
        }
    }

    return tmp;
}

async function queryOptions({hostname, port, username, password, vhost}) {
    let tmp = [];
    if (hostname !== "" && port !== "" && username !== "" && password !== "" && vhost !== "") {
        let encodedVhost = encodeURIComponent(vhost);
        let queueRequest = axios.get(getConnString(hostname, port) + `/api/queues/${encodedVhost}`, {
            auth: {
                username: username,
                password: password
            },
            headers: {
                "Content-Type": "application/json",
            }
        });
        let exchangeRequest = axios.get(getConnString(hostname, port, username, password) + `/api/exchanges/${encodedVhost}`, {
            auth: {
                username: username,
                password: password
            },
            headers: {
                "Content-Type": "application/json",
            }
        });
        let responses = await Promise.all([queueRequest, exchangeRequest]);

        let queueResponse = responses[0];
        let exchangeResponse = responses[1];

        if (queueResponse.status === 200) {
            for (let queue of queueResponse.data) {
                if (queue['name'] !== "") {
                    tmp.push({
                        type: "Queue",
                        name: queue['name']
                    })
                }
            }
        }
        if (exchangeResponse.status === 200) {
            for (let exchange of exchangeResponse.data) {
                if (exchange['name'] !== "" && !exchange['name'].includes("amq.")) {
                    tmp.push({
                        type: "Exchange",
                        exchangeType: exchange["type"],
                        name: exchange['name']
                    })
                }
            }
        }
    }
    return tmp;
}

function getConnString(hostname, port) {
    return `http://${hostname}:${port}`
}

export {queryVHosts, queryOptions, queryPort}