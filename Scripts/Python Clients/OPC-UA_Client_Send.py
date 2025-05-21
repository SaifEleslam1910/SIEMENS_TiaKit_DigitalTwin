from opcua import Client
import socket
import json

# Configuration
server_url = "opc.tcp://192.168.0.1:4840"
node_id_L = "ns=3;s=\"MOTOR LEFT\""
node_id_R = "ns=3;s=\"MOTOR RIGHT\""

# Connect to OPC UA server
client = Client(server_url)
client.connect()
print("✔ Connected to OPC UA server")

# Get nodes
nodeML = client.get_node(node_id_L)
nodeMR = client.get_node(node_id_R)

# Start TCP server for commands
host = '192.168.0.10'
port = 5001
with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
    s.bind((host, port))
    s.listen()
    print(f"TCP server listening on {host}:{port} for commands")
    conn, addr = s.accept()
    with conn:
        print(f"Connected by {addr} for commands")
        while True:
            try:
                # Receive data from TCP
                data = conn.recv(1024)
                if not data:
                    break
                # Parse JSON
                commands = json.loads(data.decode('utf-8'))
                for key, value in commands.items():
                    if key == "MOTOR_LEFT":
                        # Set value (assume boolean)
                        if isinstance(value, bool):
                            nodeML.set_value(value)
                            print(f"✔ Node {node_id_L} updated to: {value}")
                        else:
                            print(f"⚠ Invalid value for MOTOR_LEFT: {value}, expected boolean")
                    elif key == "MOTOR_RIGHT":
                        if isinstance(value, bool):
                            nodeMR.set_value(value)
                            print(f"✔ Node {node_id_R} updated to: {value}")
                        else:
                            print(f"⚠ Invalid value for MOTOR_RIGHT: {value}, expected boolean")
                    else:
                        print(f"⚠ Unknown command: {key}")
            except json.JSONDecodeError as e:
                print(f"⚠ Invalid JSON: {e}")
            except Exception as e:
                print(f"❌ Error setting value: {e}")

client.disconnect()
print("✔ Disconnected from server")