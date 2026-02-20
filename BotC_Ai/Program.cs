//---------------------------------------------------------------------------------------------------------------------- 
//                                                  TESTING SETUP
//----------------------------------------------------------------------------------------------------------------------
/*Gameboard brett = new Gameboard(File.ReadAllText(Variables.boardConfig));
var pathtest = AiController.PathToCheckpoint(brett.Tiles);
                                
AiController.FindCheckpoints(brett);
                                
// Let Ai calculate best move (may be slow)
var cardstest = AiController.SelectCards(pathtest);
*/

//---------------------------------------------------------------------------------------------------------------------- 
//                                                   BOT SETUP
//---------------------------------------------------------------------------------------------------------------------
        DotNetEnv.Env.Load();
        var ip = Environment.GetEnvironmentVariable("IP");
        var port = Environment.GetEnvironmentVariable("PORT");
        
        SocketHandler.Start(ip,port);
        
        //Waiting for Lobby
        Console.WriteLine("Connecting...\n");
        System.Threading.Thread.Sleep(1000);
        while (MessageReceiver.CurrentState == gameStateEnum.Joining)
        {
                System.Threading.Thread.Sleep(3000);
        }
//---------------------------------------------------------------------------------------------------------------------- 
//                                                  LOBBY PHASE
//---------------------------------------------------------------------------------------------------------------------- 
        
        //Wait for Character Selection
        while(MessageReceiver.CurrentState == gameStateEnum.InLobby)
        {
                Console.WriteLine("Sitting in Lobby...\n");
                System.Threading.Thread.Sleep(3000);
            
        }
//---------------------------------------------------------------------------------------------------------------------- 
//                                             CHARACTER SELECTION
//---------------------------------------------------------------------------------------------------------------------- 

        //Wait for Game to start
        
        while (MessageReceiver.CurrentState == gameStateEnum.SelectingCharacter)
        {
                Console.WriteLine("Selecting Character...\n");
                System.Threading.Thread.Sleep(3000);
            
        } 

//---------------------------------------------------------------------------------------------------------------------- 
//                                               GAME START
//---------------------------------------------------------------------------------------------------------------------- 

        if (MessageReceiver.CurrentState == gameStateEnum.InGame)
        {
                Gameboard board = new Gameboard(Variables.boardConfigJSON);
                AiController._gameboard = board;
                AiController.FindCheckpoints();
                while (MessageReceiver.CurrentState == gameStateEnum.InGame)
                {
                        if (MessageReceiver.receivedCardOffer)
                        {
                                //Pathfind to Checkpoint
                                var path = AiController.PathToCheckpoint(board.Tiles);
                                
                                //Find Walls, Holes, Eye to dodge
                                
                                // Let Ai calculate best move (may be slow)
                                var cards = AiController.SelectCards2();
                                
                                // Send cardChoice to Server
                                MessageReceiver.SendCardChoice(cards);

                                MessageReceiver.receivedCardOffer = false;
                        }
                        
                        if ((board.NumberOfCheckPoints - AiState.ReachedCheckpoints) < AiController.checkPointsQueue.Count)
                        {
                                AiController.UpdateCheckpoints();
                        }

                        if (MessageReceiver.receivedGameState)
                        { 
                                var path = AiController.PathToCheckpoint(board.Tiles);
                                AiController.DrawBoard(board, path);
                                MessageReceiver.receivedGameState = false;
                        }
                        
                        if (MessageReceiver.CurrentState == gameStateEnum.Paused)
                        {
                                AiController.paused();
                        }
                }
        }