import {ConnectionBuilder} from 'electron-cgi';
import {ipcMain} from 'electron';
import {queryOptions, queryPort, queryVHosts} from "./rabbitQuery";

let cgi = undefined;

ipcMain.handle("NewAction", async (event, args) => {
    let id = args["id"];
    try {
        let response = await cgi.send("NewAction", args);
        if (response) {
            cgi.on(id, async (message) => event.sender.send(id, message));
        }
        return {status: true};
    } catch (e) {
        return {status: false, message: e}
    }

});
ipcMain.handle("TerminateAction", async (_, id) => {
    try {
        await cgi.send("TerminateAction", id);
        return {status: true};
    } catch (e) {
        return {status: false, message: e}
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
        b.connectTo("./LettuceIO/LettuceIO.BE.exe")
    }
    cgi = b.build();

}

export {initComm, cgi}
