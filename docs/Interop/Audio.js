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

        play = (loop, volume) => {
            this.sound.loop = loop;
            this.sound.volume = volume;
            this.sound.play();
        }

        pause = () => {
            this.sound.pause();
            this.sound.currentTime = 0;
        }

        stop = () => {
            this.sound.pause();
        }

        setVolume = volume => {
            this.sound.volume = volume;
		}
    },

    loadSound: source => {
        if (!audio.sounds.hasOwnProperty(source)) {
            let sound = new audio.Sound(source);
            audio.sounds[source] = sound;
        }
    },

    play: (source, loop, volume) => {
        audio.sounds[source].play(loop, volume);
    },

    pause: source => {
        audio.sounds[source].stop();
    },

    stop: source => {
        audio.sounds[source].pause();
    }
}