using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

public class MessageReceiver
{
    public static gameStateEnum CurrentState = gameStateEnum.Unconnected;
    //Used to name 2nd/3rd... ai 
    static int KiCounter = 1; 
    //checks message type and handles the message accordingly by starting the <messageType>Received mathod
    public static bool receivedCardOffer = false;
    public static bool receivedGameState = false;
    
    /// <summary>
    /// Extracts the messages messageType and calls the ...Received Method for the messageType.
    /// If the message does not fit the protocol or has a messageType that clients should not receive
    /// the handleUnfittingMessage Method is called instead.
    /// </summary>
    /// <param name="message"> The message that needs to be processed. </param>
    public static void HandleMessage(string message)
    {
        JObject messageJson;
        messageEnum messageType;
        
        try
        {
            messageJson = JObject.Parse(message);
            messageType = Enum.Parse<messageEnum>(messageJson.GetValue("message").ToObject<string>());
            if (messageType != messageEnum.CARD_EVENT)
            {
                Console.WriteLine("message received: " + message);
            }
        }
        catch (Exception)
        {
            HandleUnfittingMessage(message);
            return;
        }

        switch (messageType)
        {
            case messageEnum.ERROR:
                ErrorReceived(messageJson);
                break;
            case messageEnum.PAUSED:
                PausedReceived(messageJson);
                break;
            case messageEnum.GAME_END:
                GameEndReceived(messageJson);
                break;
            case messageEnum.CARD_EVENT:
                // CardEventReceived(messageJson);
                break;
            case messageEnum.CARD_OFFER:
                CardOfferReceived(messageJson);
                break;
            case messageEnum.GAME_START:
                GameStartReceived(messageJson);
                break;
            case messageEnum.GAME_STATE:
                GameStateReceived(messageJson);
                break;
            case messageEnum.SHOT_EVENT:
                // ShotEventReceived(messageJson);
                break;
            case messageEnum.RIVER_EVENT:
                // RiverEventReceived(messageJson);
                break;
            case messageEnum.EAGLE_EVENT:
                // EagleEventReceived(messageJson);
                break;
            case messageEnum.ROUND_START:
                RoundStartReceived(messageJson);
                break;
            case messageEnum.HELLO_CLIENT:
                HelloClientReceived(messageJson);
                break;
            case messageEnum.CHARACTER_OFFER:
                CharacterOfferReceived(messageJson);
                break;
            case messageEnum.INVALID_MESSAGE:
                InvalideMessageResponseReceived(messageJson);
                break;
            case messageEnum.PARTICIPANTS_INFO:
                ParticipantsInfoReceived(messageJson);
                break;
            default:
                HandleUnfittingMessage(message);
                break;
        }
    }
    
    /// <summary>
    /// Writes a log-message noting that an unfitting message has been recieved.
    /// </summary>
    /// <param name="message"> message that has been received</param>
    private static void HandleUnfittingMessage(string message)
    {
        Console.WriteLine("unfitting message received");
    }
    
    private static void ErrorReceived(JObject messageJson)
    {
        try
        {
            JObject data = (JObject) messageJson.GetValue("data");
            int errorCode = (int) data.GetValue("errorCode");

            switch (errorCode)
            {
                case 0:
                case 1:
                case 2:
                    Console.WriteLine("Name already Taken");
                    HelloServer();
                    break;
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 10:
                    Console.WriteLine($"received errorCode {errorCode}");
                    break;
                case 9:
                    if (CurrentState == gameStateEnum.SelectingCharacter)
                    {
                    }
                    else
                    {
                        Console.WriteLine("received errorCode 9 at wrong time");
                    }
                    break;
                default:
                    Console.WriteLine("received error with unknown error code");
                    break;
            }
        }
        catch (Exception)
        {
            Console.WriteLine("recived faulty error message");
            //
        }
        
    }
    
    private static void PausedReceived (JObject messageJson)
    {
        Console.WriteLine("received PAUSED");
        
        //: add new GameState to allow pause only in Zugrunde 
        try
        {
            JObject data = (JObject)messageJson.GetValue("data");
            var pauseValue = data.SelectToken("pause")?.ToObject<bool>();
            Console.Write(pauseValue);

            if (pauseValue == true)
            {
                if (CurrentState == gameStateEnum.InGame)
                {
                    CurrentState = gameStateEnum.Paused;
                }
                else
                {
                    Console.WriteLine("PAUSED not accepted");
                }
            }
            else if (pauseValue == false)
            {
                if (CurrentState == gameStateEnum.Paused)
                {
                    CurrentState = gameStateEnum.InGame;
                }
                else
                {
                    Console.WriteLine("UNPAUSE not accepted");
                }
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Error in PAUSED");
            //
        }
    }
    
    private static void GameEndReceived (JObject messageJson)
    {
        if (CurrentState != gameStateEnum.InGame)
        {
            Console.WriteLine("received GAME_END at wrong time");
            return;
        }
        
        Variables.reconnectPossible = false;
        SocketHandler.ClosedOnPurpose = true;

        try
        {
            JObject data = (JObject)messageJson.GetValue("data");
        }
        catch (Exception)
        {
            Console.WriteLine("Error in GAME_END");
        }

        CurrentState = gameStateEnum.Unconnected;
        SocketHandler.Disconnect();
    }
    
    private static void CardEventReceived (JObject messageJson)
    {
        ParseMultiplePlayerAndBoardstates(messageJson);
    }
    
    private static void CardOfferReceived (JObject messageJson)
    {
        try
        {
            //assigned by taking the value of the "data" property in the messageJson
            JObject data = (JObject)messageJson.GetValue("data");
            Console.WriteLine(data);
            var cards = data.SelectToken("cards")?.ToObject<cardEnum []>();
        
            // Delete old cards
            AiState.ReceivedCards.Clear();
            
            // Add new cards
            for (int i = 0; i < cards.Length; i++)
            {
                AiState.ReceivedCards.Add(cards[i]);
            }
            
            // Serialize the message and send it to the server
            string serializedMessage = data.ToString();
            Console.WriteLine("received CARD_OFFER");
            
            // set receivedCardOffer to true to signalize Program.cs to send CardChoice
            receivedCardOffer = true;
        }
        catch (Exception)
        {
            Console.WriteLine("Error in CARD_OFFER,");
            //
        }
    }
    
    private static void GameStartReceived (JObject messageJson)
    {
        Variables.reconnectPossible = true;
        
        if (CurrentState == gameStateEnum.InLobby)
        {
            
        }
        else
        {
            Console.WriteLine("received GAME_START at wrong time");
        }
    }
    
    private static void GameStateReceived (JObject messageJson)
    {
        if (CurrentState == gameStateEnum.SelectingCharacter || CurrentState == gameStateEnum.InGame)
        {
            ParsePlayerstates(messageJson);
            ParseBoardstate(messageJson);
            CurrentState = gameStateEnum.InGame;
            
        }
        else
        {
            Console.WriteLine("received GAME_STATE at wrong time");
        }
        
        receivedGameState = true;
    }
    
    private static void ShotEventReceived (JObject messageJson)
    {
        Console.WriteLine("received SHOT_EVENT");

        try
        {
            JObject data = (JObject) messageJson.GetValue("data");

           ParsePlayerstates(messageJson);

            System.Threading.Thread.Sleep((int) Variables.inGameDelayMillisec / 2);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //
        }
    }
    
    private static void RiverEventReceived (JObject messageJson)
    {
        ParseMultiplePlayerAndBoardstates(messageJson);
    }
    
    private static void EagleEventReceived (JObject messageJson)
    {
        if (CurrentState == gameStateEnum.InGame)
        {
            ParsePlayerstates(messageJson);
        }
        else
        {
            Console.WriteLine("recieved EAGLE_EVENT at wrong time");
        }
    }
    
    private static void RoundStartReceived (JObject messageJson)
    {
        if (CurrentState == gameStateEnum.InGame)
        {
            ParsePlayerstates(messageJson);
        }
        else
        {
            Console.WriteLine("received ROUND_START at wrong time");
        }
    }
    
    private static void HelloClientReceived (JObject messageJson)
    {
        if (CurrentState != gameStateEnum.Joining && CurrentState != gameStateEnum.Reconnecting)
        {
            Console.WriteLine("received HELLO_CLIENT at wrong time");
            return;
        }

        try
        {
            
            Console.WriteLine("received HELLO_CLIENT");
            JObject data = (JObject) messageJson.GetValue("data");
            
            JToken boardConfig = data.GetValue("boardConfig");
            JToken gameConfig = data.GetValue("gameConfig");
            Variables.reconnectToken = data.GetValue("reconnectToken").ToString();
            
            try
            {
                if (boardConfig.IsValid(JSchema.Parse(File.ReadAllText(Path.Combine("BotC_Ai","Schemas", "config","boardConfig.schema.json")))))
                {
                    Variables.boardConfigJSON = boardConfig.ToString();
                }
                else
                {
                    Console.WriteLine("ERROR: boardConfig invalid");
                    //
                }
            
                if (gameConfig.IsValid(JSchema.Parse(File.ReadAllText(Path.Combine("BotC_Ai","Schemas", "config","gameConfig.schema.json")))))
                {
                    Variables.gameConfig = gameConfig.ToString();
                }
                else
                {
                    Console.WriteLine("ERROR: gameConfig invalid");
                    //
                }

                Variables.inGameDelayMillisec = JObject.Parse(Variables.gameConfig).GetValue("serverIngameDelay")
                    .ToObject<float>();
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem with validation: " + e);
            }

            if (CurrentState == gameStateEnum.Joining)
            {
                CurrentState = gameStateEnum.InLobby;
                Console.WriteLine("\n");
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.Write("Current Gamestate: " + MessageReceiver.CurrentState.ToString());
                Console.ResetColor();
                Console.WriteLine("\n");
            }
            else
            {
                CurrentState = gameStateEnum.InGame;
            }
            
            
        }
        catch (Exception e)
        {
            Console.WriteLine("Error in HELLO_CLIENT:" + e);
           
        }
        
    }

    public static void HelloServer()
    {
        String name = Environment.GetEnvironmentVariable("NAME");
        AiState.Name = name + " [" + KiCounter + "]";
        string message = "{\n" +
                         "    \"message\": \"HELLO_SERVER\",\n" +
                         "    \"data\": {\n" +
                         "        \"role\": \"AI\",\n" +
                         "        \"name\": \""+ AiState.Name +"\"\n" +
                         "        }\n" +
                         "}";

            
        SocketHandler.WriteMessage(message);
        Console.WriteLine("Sent HELLO_SERVER as: " + AiState.Name);
        KiCounter++;
        
        int delay = Convert.ToInt16(Environment.GetEnvironmentVariable("DELAY"));
        System.Threading.Thread.Sleep(delay);
        
        string message2 = "{\n" +
                          "    \"message\": \"PLAYER_READY\",\n" +
                          "    \"data\": {\n" +
                          "        \"ready\": true \n" +
                          "        }\n" +
                          "}";

        SocketHandler.WriteMessage(message2);
        Console.WriteLine("AI SUPER PLAYER " + (KiCounter-1) + " READY");
    }

    private static void CharacterOfferReceived (JObject messageJson)
    {
        if (CurrentState == gameStateEnum.InLobby)
        {
            CurrentState = gameStateEnum.SelectingCharacter;
            Console.WriteLine("\n");
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("Current Gamestate: " + MessageReceiver.CurrentState.ToString());
            Console.ResetColor();
            Console.WriteLine("\n");
        }
        
        Console.WriteLine("received CHARACTER_OFFER");
        
        try
        {
            JObject data = (JObject)messageJson.GetValue("data");
            
            JArray characters = (JArray)data.GetValue("characters");
            string characterOne = characters[0].ToString();
            
            string message = "{\n" +
                             "    \"message\": \"CHARACTER_CHOICE\",\n" +
                             "    \"data\": {\n" +
                             "              \"characterChoice\": \"" + characterOne + "\"\n" +
                             "    }\n" +
                             "}";

            
            SocketHandler.WriteMessage(message);
            Console.WriteLine("Character selected: " + characterOne);
            Console.WriteLine("\n");
            Console.BackgroundColor = ConsoleColor.Red;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write("Current Gamestate: " + MessageReceiver.CurrentState.ToString());
            Console.ResetColor();
            Console.WriteLine("\n");
        }
        catch (Exception)
        {
            Console.WriteLine("Error in CHARACTER_OFFER");
        }
        
        
    }
    
    private static void InvalideMessageResponseReceived (JObject messageJson)
    {
        Console.WriteLine("ERROR: CONNECTION WAS CLOSED DUE TO AN INVALID MESSAGE BEING SENT BY THE CLIENT");
        Variables.reconnectPossible = false;
    }
    
    private static void ParticipantsInfoReceived (JObject messageJson)
    {
        Console.WriteLine("recieved PARTICIPANTS_INFO");
        if (CurrentState != gameStateEnum.InLobby)
        {
            return;
        }

        try
        {
            JObject data = (JObject) messageJson.GetValue("data");

        }
        catch (Exception)
        {
            Console.WriteLine("Error in PARTICIPANTS_INFO");
        }
    }

    private static void ParsePlayerstates(JObject messageJson)
    {
        try
        {
            JObject data = (JObject)messageJson.GetValue("data");

            JArray playerStatesJArray = (JArray) data.GetValue("playerStates"); 
            
            // Get new state of Ai and update AiState
            foreach (JObject playerState in playerStatesJArray)
            {
                if (playerState.GetValue("playerName").ToString().Equals(AiState.Name))
                {
                    AiState.UpdateAiState(playerState);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("ParsePlayerstates: " + e);
        }
    }
    
    private static void ParseBoardstate(JObject messageJson)
    {
        try
        {
            JObject data = (JObject)messageJson.GetValue("data");

            string boardState = data.GetValue("boardState").ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    private static void ParseMultiplePlayerAndBoardstates(JObject messageJson)
    {
        try
        {
            JObject data = (JObject)messageJson.GetValue("data");

            string[][] playerStates2D = data.GetValue("playerStates").ToObject<string[][]>();
            string[] boardStates = data.GetValue("boardStates").ToObject<string[]>();

            float delay = Variables.inGameDelayMillisec / (boardStates.Length + 1);

            for (int i = 0; i < boardStates.Length; i++)
            {
                string[] playerStates = playerStates2D[i];
                string boardState = boardStates[i];

                if (i < boardState.Length - 1)
                {
                    System.Threading.Thread.Sleep((int) delay);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            //
        }
    }

    public static void SendCardChoice(List<cardEnum> cards)
    {
        try
        {
            string message = "{\n" +
                             "      \"message\": \"CARD_CHOICE\",\n" +
                             "      \"data\": {\n" +
                             "          \"cards\": [\n" +
                             "              \"" + cards[0] + "\",\n" +
                             "              \"" + cards[1] + "\",\n" +
                             "              \"" + cards[2] + "\",\n" +
                             "              \"" + cards[3] + "\",\n" +
                             "              \"" + cards[4] + "\"\n" +
                             "          ]\n" +
                             "      }\n" +
                             "}";

            SocketHandler.WriteMessage(message);
            Console.WriteLine("Sent CardChoice: " + message);
        }
        catch (Exception e)
        {
            Console.WriteLine("SendCardChoice: "+e);
        }
    }
}
//---------------------------------------------------------------------------------------------------------------------- 
//                                                 Messages
//----------------------------------------------------------------------------------------------------------------------
/*
 * BoardConfig ✓
 * GameConfig ✓
 *
 * Hello Server ✓
 * Hello Client ✓
 * Player Ready ✓
 * Game Start ✓
 * Character Offer ✓
 * Character Choice ✓
 *
 * Card Offer ✓
 * Card Choice
 * Round Start ✓
 * Card Event ✓
 * (Shot Event)
 * (River Event)
 * (Eagle Event)
 * Game State ✓
 * Game End ✓
 * Paused ✓
 */
