export function readName() {
    return localStorage.getItem("player-name");
}

export function setName(name) {
    console.log("Setting name to: " + name);
    localStorage.setItem("player-name", name);
}