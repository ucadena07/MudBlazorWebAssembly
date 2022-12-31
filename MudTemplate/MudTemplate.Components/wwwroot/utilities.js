// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.

//export function showPrompt(message) {
//  return prompt(message, 'Type anything here');
//}

function my_function(message) {
    console.log("from utilities " + message)
}

var arrow_function = (message) => {
    console.log("from utilities " + message)
}


function dotNetInstanceInovation(donetHelper) {
    donetHelper.invokeMethodAsync("IncrementCount")
}

function initializeInactivityTimer(dotnetHelper) {
    var timer
    document.onmousemove = resetTimer;
    document.onkeypress = resetTimer;

    function resetTimer() {
        clearTimeout(timer)
        timer = setTimeout(logout, 1000 * 60 * 10)
    }

    function logout() {
        document.onmousemove = null;
        document.onkeypress = null;
        dotnetHelper.invokeMethodAsync("Logout")
    }

}

function clearMouseEvents()
{
    document.onmousemove = null;
    document.onkeypress = null;
}

function setLocalStorage(key, value) {
    localStorage[key] = value
}

function getFromLocalStorage(key) {
    return localStorage[key]
}