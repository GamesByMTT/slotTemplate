using Newtonsoft.Json.Linq;
using SocketIOClient.Newtonsoft.Json;
using SocketIOClient;
using System;
using UnityEngine;
using UnityEditor.VersionControl;

public class SocketManger : MonoBehaviour
{

    [SerializeField] private SocketIOUnity socket;



    // Start is called before the first frame update
    void Start()
    {
        var TokenObj = new { token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjY2YTM5ZGE0ZDRlOGJiMDFjNTNjNGE4MCIsInVzZXJuYW1lIjoidmFpYmhhdiIsInJvbGUiOiJwbGF5ZXIiLCJpYXQiOjE3MjI4NTMxMzEsImV4cCI6MTcyMzQ1NzkzMX0.4gWo3FFo3RaLvLzDCiedXPacqV2GjpTWxuXXgV_C83k" };
        //TODO: check the Uri if Valid.
        
        var uri = new Uri("http://localhost:5000/");
        
        socket = new SocketIOUnity(uri, new SocketIOOptions
        { 
             Auth = TokenObj,
             Transport = SocketIOClient.Transport.TransportProtocol.WebSocket,
             
        });
        
        socket.JsonSerializer = new NewtonsoftJsonSerializer();

        ///// reserved socketio events
        socket.OnConnected += (sender, e) =>
        {
            Debug.Log("socket.OnConnected");
        };
        socket.OnPing += (sender, e) =>
        {
            Debug.Log("Ping");
        };
        socket.OnPong += (sender, e) =>
        {
            Debug.Log("Pong: " + e.TotalMilliseconds);
        };
        socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("disconnect: " + e);
        };
        socket.OnReconnectAttempt += (sender, e) =>
        {
            Debug.Log($"{DateTime.Now} Reconnecting: attempt = {e}");
        };
       // socket.On("message", resposeMessages);
        socket.On("message", resposeMessages);
        ////

        Debug.Log("Connecting...");
        socket.Connect();

        socket.OnUnityThread("spin", (data) =>
        {
          
        });


        socket.OnAnyInUnityThread((name, response) =>
        {
            //            ReceivedText.text += "Received On " + name + " : " + response.GetValue().GetRawText() + "\n";
        });
    }

    private void resposeMessages(SocketIOResponse Data)
    {
       //Data = Data.Trim();
        //var object = JToken.Parse(Data);
    }

    public static bool IsJSON(string str)
    {
        if (string.IsNullOrWhiteSpace(str)) { return false; }
        str = str.Trim();
        if ((str.StartsWith("{") && str.EndsWith("}")) || //For object
            (str.StartsWith("[") && str.EndsWith("]"))) //For array
        {
            try
            {
                var obj = JToken.Parse(str);
                return true;
            }
            catch (Exception ex) //some other exception
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    public void EmitSpin()
    {
        socket.Emit("spin");
    }
    }