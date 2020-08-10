import {ConnectionBuilder} from 'electron-cgi'
import {ipcMain} from 'electron'

let cgi = undefined;

ipcMain.handle("NewAction", async (event, args) => {
    let id = args["id"];
    cgi.on(id, async (e, a) => event.sender.send(id, a));
    return await cgi.send("NewAction", args);
})
ipcMain.handle("TerminateAction", async (event, args) => {
    await cgi.send("TerminateAction", args)
})

export default function (isDevelopment) {
    let b = new ConnectionBuilder();
    if (isDevelopment) {
        b.connectTo("dotnet", "run", "--project", "./Lettuce.DotNet")
    } else {
        throw "Currently only supporting dev";
    }
    cgi = b.build();
}
