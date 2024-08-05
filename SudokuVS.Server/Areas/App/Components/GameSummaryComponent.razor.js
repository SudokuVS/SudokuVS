let elementsToRegister = {};
let elementsToUpdate = undefined;
let millisecondsInAnHour = 60 /* minutes */ * 60 /* seconds */ * 1000 /* milliseconds */;

function formatDate(date) {
    const minutes = date.getMinutes();
    const minutesStr = minutes >= 10 ? minutes.toString() : "0" + minutes;

    const seconds = date.getSeconds();
    const secondsStr = seconds >= 10 ? seconds.toString() : "0" + seconds;

    if (date.getTime() >= millisecondsInAnHour) {
        const hours = Math.trunc(date.getTime() / millisecondsInAnHour);
        const hoursStr = hours >= 10 ? hours.toString() : "0" + hours;

        return hoursStr + ":" + minutesStr + ":" + secondsStr;
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
                const dateStr = formatDate(new Date(new Date() - value.date));

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