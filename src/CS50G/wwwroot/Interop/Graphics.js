var graphics = {
    canvas: null,

    initialize: canvas => {
        graphics.canvas = canvas;
	},

	newFont: (name, source) => {
		let font = new FontFace(name, `url(${source})`);

		font.load().then(() => {
			document.fonts.add(font);
		});
	},
    apply: command => {
        let context = graphics.canvas.getContext('2d');
        context.imageSmoothingEnabled = false;
        context.textBaseline = "top";

		eval(command);
	},
    wrapText: (context, text, x, y, maxWidth, lineHeight) => {
        let words = text.split(' ');
        let line = '';

        for (var n = 0; n < words.length; n++) {
            var testLine = line + words[n] + ' ';
            var metrics = context.measureText(testLine);
            var testWidth = metrics.width;
            if (testWidth > maxWidth && n > 0) {
                context.fillText(line, x, y);
                line = words[n] + ' ';
                y += lineHeight;
            }
            else {
                line = testLine;
            }
        }
        context.fillText(line, x, y);
    }
}