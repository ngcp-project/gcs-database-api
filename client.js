const WebSocket = require('ws');

let counter = 0;

let webSocket = new WebSocket('ws://localhost:5135/ws');

    setTimeout(() => {
        setInterval(() => {
            webSocket.send(`Current Num: ${counter++}`);
        }, 1);
    }, 1000)    

webSocket.addEventListener("message", (event) => {
    console.log(`Database updated: ${event.data}`);
});

// let text = '{ "employees" : [' +
// '{ "firstName":"John" , "lastName":"Doe" },' +
// '{ "firstName":"Anna" , "lastName":"Smith" },' +
// '{ "firstName":"Peter" , "lastName":"Jones" } ]}';
// webSocket.onopen = () => webSocket.send(text);

