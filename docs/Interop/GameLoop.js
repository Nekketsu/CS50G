var gameLoop = {
    instance: null,
    animation: 0,

    initialize: instance => {
        gameLoop.instance = instance;
    },

    run: () => {
        gameLoop.animation = window.requestAnimationFrame(gameLoop.update);
    },

    update: timeSpan => {
        gameLoop.animation = window.requestAnimationFrame(gameLoop.update);
        gameLoop.instance.invokeMethodAsync('Update', timeSpan);
    },

    stop: () => {
        window.cancelAnimationFrame(gameLoop.animation);
	}
}