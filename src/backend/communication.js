import {ConnectionBuilder} from 'electron-cgi'
import {ipcMain} from 'electron'
import {queryOptions, queryVHosts} from "./rabbitQuery";

let cgi = undefined;

ipcMain.handle("NewAction", async (event, args) => {
    let id = args["id"];
    cgi.on(id, async (e, a) => event.sender.send(id, a));
    return await cgi.send("NewAction", args);
});
ipcMain.handle("TerminateAction", async (event, args) => {
    await cgi.send("TerminateAction", args)
});

ipcMain.handle("queryVhost", async (event, args) => {
    return await queryVHosts(args['hostname'], args['port'], args['user'], args['password'])
})

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
