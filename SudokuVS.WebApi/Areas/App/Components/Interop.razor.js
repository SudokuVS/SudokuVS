export class Interop {
    keyupCallbackInstance;

    constructor(interopKey) {
        this.interopKey = interopKey
    }

    addKeyupEventListener() {
        this.keyupCallbackInstance = args => this.keyupCallback(args);
        window.addEventListener("keyup", this.keyupCallbackInstance)
    }

    removeKeyupEventListener() {
        window.removeEventListener("keyup", this.keyupCallbackInstance)
    }

    keyupCallback(args) {
        DotNet.invokeMethodAsync("SudokuVS.WebApi", "FireKeyupEvent", this.interopKey, {
            key: args.key,
            code: args.code,
            location: args.location,
            repeat: args.repeat,
            ctrlKey: args.ctrlKey,
            shiftKey: args.shiftKey,
            altKey: args.altKey,
            metaKey: args.metaKey,
            type: args.type,
        });
    }
}

export function createInteropInstance(key) {
    return new Interop(key);
}

window.Interop = Interop;