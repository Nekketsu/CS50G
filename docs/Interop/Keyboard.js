var keyboard = {
    instance: null,
    canvas: null,

    initialize: (instance, canvas) => {
        keyboard.instance = instance;
        keyboard.canvas = canvas;

        window.addEventListener("keydown", keyboard.onKeyDown);
        window.addEventListener("keyup", keyboard.onKeyUp);
    },

    dispose: () => {
        window.removeEventListener("keydown", keyboard.onKeyDown);
        window.removeEventListener("keyup", keyboard.onKeyUp);
    },

    onKeyDown: e => {
        keyboard.instance.invokeMethodAsync('OnKeyDown', e.code);
    },

    onKeyUp: e => {
        keyboard.instance.invokeMethodAsync('OnKeyUp', e.code);
    }
}