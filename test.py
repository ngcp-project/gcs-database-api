import pika, json 

conn = pika.BlockingConnection(pika.ConnectionParameters('localhost'))
channel = conn.channel()

data = {
        "key": "Vehicle 2",
        "speed": 1.1,
        "pitch": 2.2,
        "yaw": 3.3,
        "roll": 4.4,
        "altitude": 5.5,
        "batteryLife": 6.6,
        "currentPosition": {
            "latitude": 7.7,
            "longitude": 8.8
        },
        "vehicleStatus": 0
}


channel.queue_declare(queue='hello')
channel.basic_publish(
    exchange='',
    routing_key='telemetry_eru',
    body=json.dumps(data)
)

print(" [x] Sent 'Hello World!'")

conn.close()