function AstoriaExtension(viewer, options) {
    Autodesk.Viewing.Extension.call(this, viewer, options);
}

console.log(Autodesk.Viewing.Extension);

AstoriaExtension.prototype = Object.create(Autodesk.Viewing.Extensions.Measure.MeasureExtension.prototype);
AstoriaExtension.prototype.constructor = AstoriaExtension;

AstoriaExtension.prototype.load = function () {
    console.log('AstoriaExtension is loaded!');
    this.onSelectionBinded = this.onSelectionEvent.bind(this);
    this.viewer.addEventListener(Autodesk.Viewing.SELECTION_CHANGED_EVENT, this.onSelectionBinded);

    return true;
};

AstoriaExtension.prototype.unload = function () {
    this.viewer.removeEventListener(Autodesk.Viewing.SELECTION_CHANGED_EVENT, this.onSelectionBinded);
    this.onSelectionBinded = null;
    console.log('AstoriaExtension is now unloaded!');
    return true;
};

AstoriaExtension.prototype.onSelectionEvent = function (event) {
    for (var i = 0; i < event.dbIdArray.length; i++){
        //this.setColorMaterial(event.dbIdArray[i], 0xff0000);
    }

    var currSelection = this.viewer.getSelection();
    for (var i = 0; i < currSelection.length; i++) {
        console.log(currSelection[i]);
    }
};

AstoriaExtension.prototype.onGetPropertiesSuccess = function (event) {
    console.log("Success", event);
}

AstoriaExtension.prototype.onGetPropertiesFail = function (event) {
    console.log("Fail", event);
}

Autodesk.Viewing.theExtensionManager.registerExtension('Astoria.ForgeExtension', AstoriaExtension);