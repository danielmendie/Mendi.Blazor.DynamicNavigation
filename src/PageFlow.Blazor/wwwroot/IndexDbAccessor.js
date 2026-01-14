export function initialize() {
    let blazorSchoolIndexedDb = indexedDB.open(DATABASE_NAME, CURRENT_VERSION);
    blazorSchoolIndexedDb.onupgradeneeded = function () {
        let db = blazorSchoolIndexedDb.result;
        db.createObjectStore("data", { keyPath: "id" });
    }
}

let CURRENT_VERSION = 1;
let DATABASE_NAME = "BlazorDynamicNavigator";

export function set(collectionName, value) {
    let gamerIndexedDb = indexedDB.open(DATABASE_NAME, CURRENT_VERSION);

    gamerIndexedDb.onsuccess = function () {
        let transaction = gamerIndexedDb.result.transaction(collectionName, "readwrite");
        let collection = transaction.objectStore(collectionName)
        collection.put(value);
    }
}

export async function get(collectionName, id) {
    let request = new Promise((resolve) => {
        let gamerIndexedDb = indexedDB.open(DATABASE_NAME, CURRENT_VERSION);
        gamerIndexedDb.onsuccess = function () {
            let transaction = gamerIndexedDb.result.transaction(collectionName, "readonly");
            let collection = transaction.objectStore(collectionName);
            let result = collection.get(id);

            result.onsuccess = function (e) {
                resolve(result.result);
            }
        }
    });

    let result = await request;

    return result;
}

export function remove(collectionName, id) {
    let gamerIndexedDb = indexedDB.open(DATABASE_NAME, CURRENT_VERSION);

    gamerIndexedDb.onsuccess = function () {
        let transaction = gamerIndexedDb.result.transaction(collectionName, "readwrite");
        let collection = transaction.objectStore(collectionName)
        collection.delete(id);
    }
}

export function clear(collectionName) {
    let gamerIndexedDb = indexedDB.open(DATABASE_NAME, CURRENT_VERSION);

    gamerIndexedDb.onsuccess = function () {
        let transaction = gamerIndexedDb.result.transaction(collectionName, "readwrite");
        let collection = transaction.objectStore(collectionName)
        collection.clear();
    }
}