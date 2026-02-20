using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// The SocketHandler handels the direct communication with the server.
/// </summary>
public static class SocketHandler
{
    public static bool ClosedOnPurpose;
    private static ClientWebSocket _socket;
    private static CancellationTokenSource _source;
    
    /// <summary>
    /// Starts a new thread for receiving and processing incoming messages from the server.
    /// </summary>
    /// <param name="ip"> the servers ip-address </param>
    /// <param name="port"> the servers port number</param>
    /// <param name="caller"> the ISocketStarter that called the Start method</param>
    public static void Start(string ip, string port)
    {
        Thread thread = new Thread(() => Handle(ip, port));
        thread.Start();
    }
    
    /// <summary>
    /// Connects to the server using the specified ip-address and port number. Then keeps reading incoming messages and passing them to the MessageReciever.
    /// </summary>
    /// <param name="ip"> the servers ip-address </param>
    /// <param name="port"> the servers port number</param>
    /// <param name="caller"> the ISocketStarter that called the Start method</param>
    private static async Task Handle(string ip, string port)
    {
        _socket = new ClientWebSocket();
        _source = new CancellationTokenSource();
        ClosedOnPurpose = false;
        
        await ConnectToServer(ip, port);
        
        while (_socket.State == WebSocketState.Open)
        {
            //Console.WriteLine("SocketHandler reading");
            
            var message = await ReadMessage();
            if (message != null)
            {
                MessageReceiver.HandleMessage(message);
            }
        }

        while (_socket.State != WebSocketState.Aborted && _socket.State != WebSocketState.Closed)
        {
        }
        
        _socket.Dispose();
        Console.WriteLine("SocketHandler stopped");
    }
    
    /// <summary>
    /// Cancels the process of connecting to the server. 
    /// </summary> 
    public static void CancelConnect()
    {
        ClosedOnPurpose = true;
        try{
            _source.Cancel();
        }
        catch(Exception){}
    }
    
    /// <summary>
    /// Starts a websocket connection to the URI: "ws://" + IP + ":" + port + "/ws"
    /// </summary>
    /// <param name="ip"> the servers ip-address </param>
    /// <param name="port"> the servers port number</param>
    private static async Task ConnectToServer(string ip, string port)
    {
        try
        {
            //Uri uri = new Uri("ws://" + ip + ":" + port + "/ws");
            Uri uri = new Uri("ws://" + ip + ":" + port);
            Console.WriteLine("SocketHandler: connecting to: " + uri.ToString() );
            MessageReceiver.CurrentState = gameStateEnum.Joining;
            Console.WriteLine("\n");
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("Current Gamestate: " + MessageReceiver.CurrentState.ToString());
            Console.ResetColor();
            Console.WriteLine("\n");
            await _socket.ConnectAsync(uri, _source.Token);
 
            
            MessageReceiver.HelloServer();
            
        }
        catch(Exception e)
        {
            Console.WriteLine("SocketHandler: " + e);
        }
    }

    
    /// <summary>
    /// Sends the GOODBYE_SERVER message to disconnect from the server.
    /// </summary>
    public static void Disconnect()
    {
        ClosedOnPurpose = true;
        Variables.reconnectPossible = false;
        
        string message = "{\n" +
                         "    \"message\": \"GOODBYE_SERVER\",\n" +
                         "    \"data\": {}\n" +
                         "}";
        
        WriteMessage(message);
        /*try
        {
            Socket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }*/
    }
    
    /// <summary>
    /// Sends the specified message to the server.
    /// </summary>
    /// <param name="message"> the message to be sent</param>
    public static void WriteMessage(string message)
    {
        try
        {
            var bytes = Encoding.Default.GetBytes(message);
            var arraySegment = new ArraySegment<byte>(bytes);
            
            _socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
            
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            ConnectionLost();
        }
    }

    /// <summary>
    /// Reads an incoming message from the server.
    /// </summary>
    /// <returns> a Task containing the servers message or null if an error occured while reading</returns>
    private static async Task<string> ReadMessage()
    {
        try
        {
            string message = "";
            
            while (true)
            {
                var arraySegment = new ArraySegment<byte>(new byte[4096]);
                var receivedMessage = await _socket.ReceiveAsync(arraySegment, CancellationToken.None);
            
                if (receivedMessage.MessageType == WebSocketMessageType.Text)
                {
                    message = message + Encoding.Default.GetString(arraySegment).TrimEnd('\0');
                }

                if (receivedMessage.EndOfMessage)
                {
                    break;
                }
            }
            
            if (!string.IsNullOrWhiteSpace(message))
            {
                return message;
            }
            
            
            /*
            var arraySegment = new ArraySegment<byte>(new byte[4096]);
            var receivedMessage = await _socket.ReceiveAsync(arraySegment, CancellationToken.None);
            
            if (receivedMessage.MessageType == WebSocketMessageType.Text)
            {
            
                var message = Encoding.Default.GetString(arraySegment).TrimEnd('\0');
                if (!string.IsNullOrWhiteSpace(message))
                {
                    return message;
                }
            }*/
        }
        catch(Exception e)
        {
            Console.WriteLine(e);
            ConnectionLost();
        }
        return null;
        
    }


    /// <summary>
    /// Is called when the connection to the server gets closed.
    /// Checks if the disconnect was on purpose and directs the user to the reconnect scene if not.
    /// </summary>
    private static void ConnectionLost()
    {
        if (ClosedOnPurpose)
        {
            return;
        }

        int count = 1;
        try
        {
            while (count < 4)
            {
                System.Threading.Thread.Sleep(5000*count);
                reconnect();
                count++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("connection lost: " + e);
        
            Variables.disconnected = true;
        
            _socket.Dispose();
        }
        
    }

    private static void reconnect()
    {
        if (Variables.reconnectPossible)
        {
            string message = "{\n" +
                             "    \"message\": \"RECONNECT\",\n" +
                             "    \"data\": {\n" +
                             "        \"name\":" + AiState.Name + "\n" +
                             "        \"reconnectToken\":" + Variables.reconnectToken + "\n" +
                             "        }\n" +
                             "}";
        
            WriteMessage(message);
        }
    }

}