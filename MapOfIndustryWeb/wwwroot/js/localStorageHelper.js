var localStorageHelper = {
    set(key, value) {
        if (typeof value === "object") {
            value = JSON.stringify(value);
        }
        localStorage.setItem(key, value);
    },
    get(key) {
        const data = localStorage.getItem(key);
        try {
            return JSON.parse(data);
        } catch (err) {
            return data;
        }
    },
    remove(key) {
        localStorage.removeItem(key);
    }
};