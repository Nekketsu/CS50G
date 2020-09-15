var fileSystem = {
	exists: key => {
		let item = localStorage.getItem(key);
		if (item) {
			return true;
		}
		else {
			return false;
		}
	},

	write: (key, value) => {
		localStorage.setItem(key, value);
	},

	read: key => {
		return localStorage.getItem(key);
	}
}