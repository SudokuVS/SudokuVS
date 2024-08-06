let elementsToRegister = {};
let elementsToUpdate = undefined;

function formatMilliseconds(milliseconds) {
    const totalSeconds = Math.floor(milliseconds / 1000);
    const totalMinutes = Math.floor(totalSeconds / 60);
    const totalHours = Math.floor(totalMinutes / 60);

    const seconds = totalSeconds - 60 * totalMinutes;
    const secondsStr = seconds >= 10 ? seconds.toString() : "0" + seconds;

    const minutes = totalMinutes - 60 * totalHours;
    const minutesStr = minutes >= 10 ? minutes.toString() : "0" + minutes;

    if (totalHours > 0) {
        return totalHours + ":" + minutesStr + ":" + secondsStr;
    } else {
        return minutesStr + ":" + secondsStr;
    }
}

export function startTimer(containerElementId, startTime) {
    if (elementsToUpdate === undefined) {
        // first call
        elementsToUpdate = {};

        setInterval(() => {
            for (const [id, date] of Object.entries(elementsToRegister)) {
                const elements = document.getElementsByClassName(id);
                elementsToUpdate[id] = {elements, date};
            }

            elementsToRegister = {};

            for (const value of Object.values(elementsToUpdate)) {
                const dateStr = formatMilliseconds(new Date().getTime() - value.date.getTime());

                for (const element of value.elements) {
                    element.innerText = dateStr;
                }
            }
        }, 1000)
    }

    elementsToRegister[containerElementId] = new Date(startTime);
}

export function stopTimer(containerElementId) {
    if (containerElementId in elementsToRegister) {
        delete elementsToRegister[containerElementId];
    }

    if (elementsToUpdate) {
        if (containerElementId in elementsToUpdate) {
            for (const element of elementsToUpdate[containerElementId].elements) {
                element.innerText = null;
            }

            delete elementsToRegister[containerElementId];
        }
    }
}