export function setItem(key, value) {
    window.localStorage.setItem(key, JSON.stringify(value));
}

export function getItem(key) {
    const v = window.localStorage.getItem(key);
    return v ? JSON.parse(v) : null;
}

export function removeItem(key) {
    window.localStorage.removeItem(key);
}

export function clear() {
    windo.localStorage.clear();
}