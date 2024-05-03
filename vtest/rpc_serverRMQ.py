# Vehicle
import pika 
import time
import json
from Commands import Commands
from Geolocation import Coordinate, Polygon

connection = pika.BlockingConnection(pika.ConnectionParameters(host='localhost'))
channel = connection.channel(0)

channel.queue_declare(queue='rpc_queue')

def isManual(isManual:bool):
    print(f"Manual is:{isManual}")
    
def target(target:Coordinate):
    print(f"Target Coordinate is:{target}")
    
def searchArea(searchArea:Polygon):
    print(f"Search Area is:{searchArea}")
    
def subscribe_all(commands_data) -> str:
    command_dict = json.loads(commands_data)
    
    # Use the .get method to provide a default value if 'isManual' key is not found
    is_manual = command_dict.get('isManual', False)  # Assuming default False if not specified
    target_info = command_dict.get('target', {})
    target_latitude = target_info.get('latitude', 0)  # Default to 0 if not found
    target_longitude = target_info.get('longitude', 0)  # Default to 0 if not found

    search_area_info = command_dict.get('searchArea', {})
    search_area_coordinates = search_area_info.get('coordinates', [])
    search_area_coordinates_list = [
        Coordinate(latitude=coor.get('latitude', 0), longitude=coor.get('longitude', 0))
        for coor in search_area_coordinates
    ]
    
    targetCoordinate = Coordinate(target_latitude, target_longitude)
    search_area = Polygon(coordinates=search_area_coordinates_list)
    
    isManual(is_manual)
    target(targetCoordinate)
    searchArea(search_area)
    
    return "[.] Vehicle received commands from GCS and Answer: " + json.dumps(command_dict)


def on_request(ch, method, props, body):
    
    commands = body
    
    response = subscribe_all(commands)
    # print(f"[.] Vehicle received commands from GCS: {commands}")
    
    ch.basic_publish(exchange='', routing_key=props.reply_to, 
                     properties=pika.BasicProperties(correlation_id=props.correlation_id),
                     body=str(response))
    ch.basic_ack(delivery_tag=method.delivery_tag)
    last_call = time.time()

channel.basic_qos(prefetch_count=1)
channel.basic_consume(queue='rpc_queue', on_message_callback=on_request)
print(" [x] Awaiting GCS RPC requests")
channel.start_consuming()
