import {ConnectionBuilder} from 'electron-cgi'
import {ipcMain} from 'electron'

let cgi = undefined;

export default function (isDevelopment) {
    let b = new ConnectionBuilder();
    if(isDevelopment){
        b.connectTo("dotnet", "run", "--project", "./Lettuce.DotNet")
    } else{
        throw new "";
    }
    cgi = b.build();
}
