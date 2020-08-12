var keyboard = {
    instance: null,
    canvas: null,

    initialize: (instance, canvas) => {
        keyboard.instance = instance;
        keyboard.canvas = canvas;

        window.addEventListener("keydown", e => {
            keyboard.instance.invokeMethodAsync('OnKeyDown', e.code);
        });

        window.addEventListener("keyup", e => {
            keyboard.instance.invokeMethodAsync('OnKeyUp', e.code);
        });
    }
}