var graphics = {
    instance: null,
    canvas: null,

    images: {},

    initialize: (instance, canvas, superSampling) => {
        graphics.instance = instance;
        graphics.canvas = canvas;
        let context = canvas.getContext("2d");
        context.scale(superSampling, superSampling);
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
    },

    //newImage: async source => {
    //    let image;
    //    if (!graphics.images.hasOwnProperty(source)) {
    //        image = await new Promise((resolve, reject) => {
    //            let img = new Image();
    //            img.onload = () => resolve(img);
    //            img.onerror = reject;
    //            img.src = source;

    //            graphics.images[source] = img;
    //        });
    //    }
    //    else {
    //        image = graphics.images[source];
    //    }

    //    return image ? { name: source, width: image.naturalWidth, height: image.naturalHeight } : null;
    //}

    newImage: async source => {
        let image;
        if (!graphics.images.hasOwnProperty(source)) {
            image = new Image();
            image.onload = () => graphics.instance.invokeMethodAsync('NewImageCompleted', { name: source, width: image.naturalWidth, height: image.naturalHeight });
            image.onerror = () => graphics.instance.invokeMethodAsync('NewImageCompleted', null);
            image.src = source;

            graphics.images[source] = image;
        }
        else {
            image = graphics.images[source];
            graphics.instance.invokeMethodAsync('NewImageCompleted', { name: source, width: image.naturalWidth, height: image.naturalHeight });
        }
    }
}