var gameLoop = {
    instance: null,

    update: async timeSpan => {
        await gameLoop.instance.invokeMethodAsync('Update', timeSpan);
        window.requestAnimationFrame(gameLoop.update);
    },

    initialize: instance => {
        gameLoop.instance = instance;
	},

    run: () => {
        window.requestAnimationFrame(gameLoop.update);
    }
}