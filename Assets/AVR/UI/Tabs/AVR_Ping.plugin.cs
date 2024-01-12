using UnityEngine;

public class Plugin_Ping : AVR.Plugin
{
    public override void Initialize(AVR.Startup startup)
    {
        Id = "avr-ping";
        base.Initialize(startup);
    }

    float nextPing = 1f;
    public override void Update()
    {
        if (Time.time > nextPing && nextPing != 0f)
        {
            nextPing = 0f;
            
            foreach(AVR.Socket socket in AVR.SocketManager.sockets)
                socket.Ping();
            
            nextPing = Time.time + 5f;
        }
    }
}

