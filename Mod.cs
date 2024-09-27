using BepInEx;
using Photon.Pun;
using System;
using System.IO;
using System.Net.Http;
using System.Text;

[BepInPlugin("com.pxsl.selftracker", "Pxsl's Self Tracker", "1.2.0")]
public class RoomTrackerPlugin : BaseUnityPlugin
{
    // Path to the file containing the webhook URL
    private string infoFilePath;
    private string webhookUrl;
    private HttpClient httpClient;
    private bool gameStartSent = false; // To ensure we only send the game start notification once

    private void Awake()
    {
        // Adjusted the path to look in the plugin's own folder under BepInEx/plugins/SelfTracker
        string pluginFolderPath = Path.Combine(Paths.PluginPath, "SelfTracker");
        infoFilePath = Path.Combine(pluginFolderPath, "info.txt");

        // Initialize the HttpClient for sending requests
        httpClient = new HttpClient();

        // Try to load the webhook URL from the info.txt file
        LoadWebhookUrl();

        // Log a message to indicate the mod has been loaded
        Logger.LogInfo("Room Tracker Plugin Loaded.");

        // Send game start notification
        SendGameStartNotification();
    }

    private void LoadWebhookUrl()
    {
        try
        {
            // Check if the info.txt file exists in the plugin folder
            if (File.Exists(infoFilePath))
            {
                // Read the webhook URL from the file
                webhookUrl = File.ReadAllText(infoFilePath).Trim();

                // Log the loaded webhook URL
                Logger.LogInfo("Webhook URL loaded from info.txt.");
            }
            else
            {
                // Log an error if the file is missing
                Logger.LogError($"The info.txt file is missing in the SelfTracker folder. Please create it and add your Discord webhook URL.");
            }
        }
        catch (Exception e)
        {
            // Log any exceptions that occur during file reading
            Logger.LogError($"Error reading info.txt file: {e.Message}");
        }
    }

    private void Update()
    {
        // Check if we are connected to Photon and in a room, and if the webhook URL is set
        if (PhotonNetwork.InRoom && !string.IsNullOrEmpty(webhookUrl))
        {
            // Get room information
            string roomCode = PhotonNetwork.CurrentRoom.Name;
            int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
            string playerList = GetPlayerList();

            // Send the room information to the Discord webhook
            SendRoomInfoToDiscord(roomCode, playerCount, playerList);
        }
        else if (PhotonNetwork.InRoom && string.IsNullOrEmpty(webhookUrl))
        {
            Logger.LogWarning("Webhook URL is missing. Please ensure info.txt contains a valid URL.");
        }
    }

    private async void SendGameStartNotification()
    {
        if (!string.IsNullOrEmpty(webhookUrl) && !gameStartSent)
        {
            try
            {
                // Prepare the content to send to Discord
                var jsonContent = new
                {
                    content = "**Tracker Notification**\nGame has started!"
                };

                // Serialize the object to a JSON string
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonContent);

                // Create the content to be sent in the POST request
                var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

                // Send the POST request to the Discord webhook
                HttpResponseMessage response = await httpClient.PostAsync(webhookUrl, httpContent);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    Logger.LogInfo("Game start notification sent to Discord.");
                    gameStartSent = true; // Prevent multiple sends
                }
                else
                {
                    Logger.LogError($"Failed to send game start notification: {response.StatusCode}, {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException e)
            {
                // Handle network errors
                Logger.LogError($"Error sending game start notification: {e.Message}");
            }
        }
    }

    private async void SendRoomInfoToDiscord(string roomCode, int playerCount, string playerList)
    {
        try
        {
            // Prepare the content to send to Discord with more detailed info
            var jsonContent = new
            {
                content = $"**Tracker Update**\n" +
                          $"Room Code: `{roomCode}`\n" +
                          $"Player Count: `{playerCount}`\n" +
                          $"Players: `{playerList}`"
            };

            // Serialize the object to a JSON string
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(jsonContent);

            // Create the content to be sent in the POST request
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            // Send the POST request to the Discord webhook
            HttpResponseMessage response = await httpClient.PostAsync(webhookUrl, httpContent);

            // Check if the request was successful
            if (response.IsSuccessStatusCode)
            {
                Logger.LogInfo("Successfully sent room information to Discord.");
            }
            else
            {
                Logger.LogError($"Failed to send room information to Discord: {response.StatusCode}, {response.ReasonPhrase}");
            }
        }
        catch (HttpRequestException e)
        {
            // Handle network errors
            Logger.LogError($"Error sending room information to Discord: {e.Message}");
        }
    }

    private string GetPlayerList()
    {
        StringBuilder playerListBuilder = new StringBuilder();
        foreach (var player in PhotonNetwork.PlayerList)
        {
            playerListBuilder.Append(player.NickName).Append(", ");
        }

        // Remove trailing comma and space
        if (playerListBuilder.Length > 0)
        {
            playerListBuilder.Length -= 2;
        }

        return playerListBuilder.ToString();
    }

    private void OnDestroy()
    {
        // Clean up the HttpClient when the plugin is unloaded
        httpClient?.Dispose();
    }
}