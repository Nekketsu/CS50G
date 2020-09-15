var mouse = {
    instance: null,
    canvas: null,

    initialize: (instance, canvas) => {
        mouse.instance = instance;
        mouse.canvas = canvas;

        canvas.addEventListener("mousedown", mouse.onMouseDown);
        canvas.addEventListener("mouseup", mouse.onMouseUp);
    },

    dispose: () => {
        window.removeEventListener("mousedown", mouse.onMouseDown);
        window.removeEventListener("mouseup", mouse.onMouseUp);
    },

    onMouseDown: e => {
        mouse.instance.invokeMethodAsync('OnMouseDown', e.clientX, e.clientY, e.button);
    },

    onMouseUp: e => {
        mouse.instance.invokeMethodAsync('OnMouseUp', e.clientX, e.clientY, e.button);
    }
}