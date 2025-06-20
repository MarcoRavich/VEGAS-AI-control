import pythoncom
import win32com.client
import socket

# Simple OSC receiver (UDP) that calls Vegas script via COM
UDP_IP = "127.0.0.1"
UDP_PORT = 9000

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sock.bind((UDP_IP, UDP_PORT))

def call_vegas_script(method, *args):
    pythoncom.CoInitialize()
    vegas = win32com.client.Dispatch("Vegas.Application")
    ep = vegas.ScriptPortal.ScriptHost.EntryPoint()
    getattr(ep, method)(vegas, *args)

while True:
    data, addr = sock.recvfrom(1024)
    msg = data.decode()
    # Parse message, e.g., "Play", "SetTrackVolume 0 -6.0"
    tokens = msg.strip().split()
    method = tokens[0]
    args = [float(x) if '.' in x else int(x) for x in tokens[1:]]
    call_vegas_script(method, *args)