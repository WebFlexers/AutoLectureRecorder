// https://stackoverflow.com/a/65398068/15011818
document.addEventListener('mousemove', function (event) {
    let elem = event.target;
    let jsonObject =
    {
        Key: 'mousemove',
        Value: elem.name || elem.id || elem.tagName || "Unkown"
    };
    window.chrome.webview.postMessage(jsonObject);
});