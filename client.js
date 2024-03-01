const WebSocket = require('ws');

let webSocket = new WebSocket('ws://localhost:5135/ws');


// Sample Vehicle data
let vehicleData = {
        "key": "Vehicle 2",
        "speed": 1.1,
        "pitch": 2.2,
        "yaw": 3.3,
        "roll": 4.4,
        "altitude": 5.5,
        "batteryLife": 6.6,
        "lastUpdated": "00:00:00",
        "currentPosition": {
            "latitude": 7.7,
            "longitude": 8.8
        },
        "vehicleStatus": 0
};      

setTimeout(() => {
    webSocket.send(JSON.stringify(vehicleData));
}, 1000)

//let counter = 0;
// setTimeout(() => {
//     setInterval(() => {
//         webSocket.send(`Current Num: ${counter++}`);
//     }, 1);
// }, 1000)    

webSocket.addEventListener("message", (event) => {
    console.log(`Database updated: ${event.data}`);
});

