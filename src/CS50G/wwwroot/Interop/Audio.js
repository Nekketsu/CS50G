var audio = {
    sounds: {},

    Sound: class {
        constructor(source) {
            this.sound = document.createElement("audio");
            this.sound.src = source;
            this.sound.setAttribute("preload", "auto");
            this.sound.setAttribute("controls", "none");
            this.sound.style.display = "none";
            document.body.appendChild(this.sound);
        }

        play = () => {
            this.sound.play();
        }

        stop = () => {
            this.sound.pause();
        }
    },

    loadSound: source => {
        if (!audio.sounds.hasOwnProperty(source)) {
            let sound = new audio.Sound(source);
            audio.sounds[source] = sound;
        }
    },

    play: source => {
        audio.sounds[source].play();
    }
}