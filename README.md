# eegmuse

### Getting Started

1. Get connected to the Muse headband. Use bluetooth on your computer to connect to pair with the headband. If you hold down the button on the headband, it helps to engage in pairing. When light is blinking on device, then headband is ready to pair. 

2. Open a UDP port to the device. Open terminal and type in:
muse-io --preset 14 --device-search musito --osc osc.udp://localhost:5000

3. Open the Unity3D project: /Assets/LKM_Main.unity

Run it. Should work. 
