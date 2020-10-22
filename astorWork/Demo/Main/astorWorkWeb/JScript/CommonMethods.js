function GetRadWindow() {
    var oWindow = null;
    if (window.radWindow)
        oWindow = window.radWindow;
    else if (window.frameElement.radWindow)
        oWindow = window.frameElement.radWindow;
    return oWindow;
}
function CloseAndReload(Operation) {
    var oWnd = GetRadWindow();
    oWnd.BrowserWindow.refreshGrid(Operation);
    oWnd.close();
}

