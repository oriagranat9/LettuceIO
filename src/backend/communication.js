import {ConnectionBuilder} from 'electron-cgi';
import {ipcMain} from 'electron';
import {queryOptions, queryVHosts, queryPort} from "./rabbitQuery";

let cgi = undefined;

ipcMain.handle("NewAction", async (event, args) => {
    let id = args["id"];
    try {
        let response = await cgi.send("NewAction", args);
        if (response) {
            cgi.on(id, async (message) => event.sender.send(id, message));
        }
        return true;
    } catch (e) {
        return false;
    }

});
ipcMain.handle("TerminateAction", async (_, id) => {
    try {
        await cgi.send("TerminateAction", id);
        return true
    } catch (e) {
        return false
    }
});

ipcMain.handle("queryVhost", async (_, args) => {
    return queryVHosts(args)
});
ipcMain.handle("queryOptions", async (_, args) => {
    return await queryOptions(args)
});

ipcMain.handle("queryPort", async (_, args) => {
    return await queryPort(args);
});

function initComm(isDevelopment) {
    let b = new ConnectionBuilder();
    if (isDevelopment) {

        b.connectTo("dotnet", "run", "--project", "./LettuceIo.DotNet/LettuceIo.Dotnet.ConsoleHost")
    } else {
        throw "Currently only supporting dev";
    }
    cgi = b.build();

}

export {initComm, cgi}
