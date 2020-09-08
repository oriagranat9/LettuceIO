import {ConnectionBuilder} from 'electron-cgi'
import {ipcMain} from 'electron'
import {queryOptions, queryVHosts} from "./rabbitQuery";

let cgi = undefined;

ipcMain.handle("NewAction", async (event, args) => {
    let id = args["id"];
    cgi.on(id, async (e, a) => event.sender.send(id, a));
    return await cgi.send("NewAction", args);
});
ipcMain.handle("TerminateAction", async (_, args) => {
    await cgi.send("TerminateAction", args)
});

ipcMain.handle("queryVhost", async (_, args) => {
    return queryVHosts(args)
});
ipcMain.handle("queryOptions", async (_, args) => {
    return await queryOptions(args)
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
