using System.Diagnostics;

namespace KI.UnitTests;
using Xunit;

[Collection("Sequential")]
public class MessageTests
{
    public MessageTests()
    {
    }

    [Fact]
    public void CardTest()
    {
        // Arrange
        String testCards = @"{
        ""message"": ""CARD_OFFER"",
        ""data"": {""cards"": [
            ""MOVE_1"",
            ""MOVE_2"",
            ""MOVE_3"",
            ""LEMBAS"",
            ""MOVE_2"",
            ""MOVE_1"",
            ""MOVE_1""]}}";

        MessageReceiver.HandleMessage(testCards);

        List<cardEnum> expectedCards = new List<cardEnum>
        {
            cardEnum.MOVE_1, cardEnum.MOVE_2, cardEnum.MOVE_3, cardEnum.LEMBAS, cardEnum.MOVE_2, cardEnum.MOVE_1,
            cardEnum.MOVE_1
        };

        // Act
        List<cardEnum> actualCards = AiState.ReceivedCards;

        // Test
        Assert.Equal(expectedCards, actualCards);

        // CleanUp
        CleanData.CleanUpData();
    }

    [Fact]
    public void PauseTest()
    {
        // Arrange
        String testPause = @"{
        ""message"": ""PAUSED"",
        ""data"": { ""playerName"": ""Player"",
                    ""pause"": true }}";
        MessageReceiver testReciever = new MessageReceiver();
        MessageReceiver.CurrentState = gameStateEnum.InGame;

        // Act
        MessageReceiver.HandleMessage(testPause);

        // Test
        Assert.Equal(gameStateEnum.Paused, MessageReceiver.CurrentState);

        // CleanUp
        CleanData.CleanUpData();
    }

    [Fact]
    public void UnpauseTest()
    {
        // Arrange
        String testPause2 = @"{
        ""message"": ""PAUSED"",
        ""data"": { ""playerName"": ""Player"",
                    ""pause"": false }}";
        MessageReceiver.CurrentState = gameStateEnum.Paused;

        // Act
        MessageReceiver.HandleMessage(testPause2);

        // Test
        Assert.Equal(gameStateEnum.InGame, MessageReceiver.CurrentState);

        // CleanUp
        CleanData.CleanUpData();
    }

    [Fact]
    public void GamestateTest()
    {
        // Arrange
        String testGamestate = @"{
            ""message"": ""GAME_STATE"",
            ""data"": {
                ""playerStates"": [
                                  {
                                    ""playerName"": ""Player1"",
                                    ""currentPosition"": [
                1,
                1
                    ],
                ""spawnPosition"": [
                1,
                2
                    ],
                ""direction"": ""NORTH"",
                ""character"": ""FRODO"",
                ""lives"": 3,
                ""lembasCount"": 0,
                ""suspended"": 0,
                ""reachedCheckpoints"": 0,
                ""playedCards"": [
                ""MOVE_1""
                    ],
                ""turnOrder"": 1
            }
            ],
            ""boardState"": {
                ""lembasFields"": [
                {
                    ""position"": [
                    0,
                    9
                        ],
                    ""amount"": 6
                },
                {
                    ""position"": [
                    9,
                    0
                        ],
                    ""amount"": 6
                }
                ]
            },
            ""currentRound"": 69
        }
    }";


                MessageReceiver.CurrentState = gameStateEnum.SelectingCharacter;

                // Act
                MessageReceiver.HandleMessage(testGamestate);

                // Test
                Assert.Equal(gameStateEnum.InGame, MessageReceiver.CurrentState);

                // CleanUp
                CleanData.CleanUpData();
            }

        }