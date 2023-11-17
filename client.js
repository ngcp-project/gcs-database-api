const WebSocket = require('ws');

let counter = 0;

let webSocket = new WebSocket('ws://localhost:5135/ws');

let vehicleData = {
    key: "Vehicle 1"
    // speed: 1.1,
    // pitch: 2.2,
    // yaw: 3.3,
    // roll: 4.4,
    // altitude: 5.5,
    // batteryLife: 6.6,
    // lastUpdated: "00:00:00"
    //vehicleStatus: 0
}  

    setTimeout(() => {
        webSocket.send(JSON.stringify(vehicleData));
    }, 1000)
    

    // setTimeout(() => {
    //     setInterval(() => {
    //         webSocket.send(`Current Num: ${counter++}`);
    //     }, 1);
    // }, 1000)    

webSocket.addEventListener("message", (event) => {
    console.log(`Database updated: ${event.data}`);
});

// let text = '{ "employees" : [' +
// '{ "firstName":"John" , "lastName":"Doe" },' +
// '{ "firstName":"Anna" , "lastName":"Smith" },' +
// '{ "firstName":"Peter" , "lastName":"Jones" } ]}';
// webSocket.onopen = () => webSocket.send(text);

