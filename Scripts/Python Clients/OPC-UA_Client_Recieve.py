from opcua import Client
import time
import socket
import json

# Configuration
server_url = "opc.tcp://192.168.0.1:4840"
node_id_S1 = "ns=3;s=\"SENSOR1\""
node_id_Barrier = "ns=3;s=\"Barrier\""
node_id_L = "ns=3;s=\"MOTOR LEFT\""
node_id_R = "ns=3;s=\"MOTOR RIGHT\""

# Connect to OPC UA server
client = Client(server_url)
try:
    client.connect()
    print("✅ Connected to OPC UA server")

    # Get nodes
    nodeS1 = client.get_node(node_id_S1)
    nodeB = client.get_node(node_id_Barrier)
    nodeML = client.get_node(node_id_L)
    nodeMR = client.get_node(node_id_R)

    # Start TCP server for state updates
    host = '192.168.0.10'
    port = 5000
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.bind((host, port))
        s.listen()
        conn, addr = s.accept()
        with conn:
            print(f"TCP client connected for state updates: {addr}")
            while True:
                try:
                    # Read current values from OPC UA
                    current_val_S1 = nodeS1.get_value()
                    current_val_Barrier = nodeB.get_value()
                    current_val_ML = nodeML.get_value()
                    current_val_MR = nodeMR.get_value()

                    # Create state dictionary
                    state = {
                        "SENSOR1": current_val_S1,
                        "Barrier": current_val_Barrier,
                        "MOTOR_LEFT": current_val_ML,
                        "MOTOR_RIGHT": current_val_MR
                    }

                    # Send state as JSON over TCP
                    message = json.dumps(state)
                    conn.sendall((message + '\n').encode('utf-8'))

                    time.sleep(1)  # Poll every 1 second
                except Exception as e:
                    print(f"Error: {e}")
                    time.sleep(1)

except KeyboardInterrupt:
    print("\n👋 Stopped by user")
except Exception as e:
    print(f"❌ Connection error: {e}")
finally:
    client.disconnect()
    print("🔌 Disconnected from server")