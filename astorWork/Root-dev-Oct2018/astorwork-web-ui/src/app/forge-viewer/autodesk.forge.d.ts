declare module Autodesk {
    let InViewerSearch: any;

    export module Viewing {

        let SELECTION_CHANGED_EVENT: any;
        let GEOMETRY_LOADED_EVENT: any;
        let TEXTURES_LOADED_EVENT: any;
        let RENDER_PRESENTED_EVENT: any;

        let SelectionMode: {
            OVERLAYED: any;
            REGULAR: any;
            MIXED: any;
        }

        export function Initializer(options: any, callback: () => void): void;

        export class Document {
            static load(documentId: string, successCallback: (doc: Document) => void, errorCallback: (msg: string) => void): void;
            static getSubItemsWithProperties(item: Object, properties: Properties, recursive: boolean): Object[];

            getRootItem(): Object;
            getViewablePath(item: Object): string;
        }

        export class Model {
            getProperties(dbId: number, onSuccessCallback: (event: any) => any, onErrorCallback: (event: any) => any): void;
        }

        export class Extension {
            constructor(viewer: any, options: any);
        }

        export class ExtensionManager {
            registerExtension(extensionId: string, extension: Extension): void;
            registerExternalExtension(extensionId: string, extensionUrl: string): void;
            getExtension(extentionId: string): any;
        }

        let theExtensionManager: ExtensionManager;

        export class ViewingApplication {
            constructor(containerId: string, options?: any);
            getCurrentViewer(): Viewer3D;
        }

        export class Viewer3D {
            constructor(container: HTMLElement, options?: any);
            getSelectionCount(): number;
            addEventListener(eventType: any, eventHandler: (event: any) => any): void;
            search(text: string, onSuccessCallback: (event: any) => any, onErrorCallback: (event: any) => any, attributeNames: Array<string>)
            EXTENSION_LOADED_EVENT: any;
        }

        interface Properties {
            type: string;
            role: string;
        }

        interface SELECTION_CHANGED_EVENT {
            dbIdArray: Array<number>;
            fragIdsArray: Array<number>;
            nodeArray: Array<number>;
            type: string;
            model: Autodesk.Viewing.Model;
            target: any;
        }

        interface GEOMETRY_LOADED_EVENT {
            data: any;
        }

        interface TEXTURES_LOADED_EVENT{
            data: any;

        }

        let Extensions: {
            Measure: { MeasureExtension },
            Markups: { Core }
        }
    }

    export module Viewing.Private {

        export class GuiViewer3D {
            constructor(container: HTMLElement);
            start(): void;
            load(path: any): void;

            getSelectionCount(): number;

            model: any;
        }
    }
}

declare module THREE {
    export class Vector4{
        r: number;
        g: number;
        b: number;
        a: number;

        constructor(a: number, b: number, c: number, d: number)
    }
}