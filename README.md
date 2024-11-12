# Pxsl's Self Tracker - Photon Room Info Discord Bot

**Pxsl's Self Tracker** is a simple BepInEx plugin that tracks the current Photon room info in real-time and sends updates to a Discord webhook. It also notifies when the game is started. This mod is for Gorilla Tag Only, I think :)

## Features
- Sends a notification to Discord when the game starts.
- Tracks the current Photon room code, number of players, and player names.
- Sends real-time updates to a Discord channel via a webhook.
- Easy to set up and configure using an `info.txt` file.

## Requirements
- **BepInEx**: The plugin is built for games using the Unity engine with BepInEx installed.
- **Photon PUN 2**: The game must be using Photon for networking.

## Installation

1. **Download and install BepInEx** (if you haven't already):
   - [BepInEx Installation Guide](https://bepinex.github.io/bepinex_docs/master/articles/user_guide/installation/index.html)
   
2. **Download the plugin**:
   - Go to the [Releases](https://github.com/your-repo-name/releases) page of this repository and download the latest release `.zip` file.

3. **Extract the contents**:
   - Extract the downloaded `.zip` file into the `BepInEx/plugins/` folder of your game directory.
   - Your folder structure should look like this:
     ```
     BepInEx
     └── plugins
         └── SelfTracker
             ├── SelfTracker.dll
             └── info.txt
     ```

4. **Edit the `info.txt` file**:
   - Open the `info.txt` file located in the `SelfTracker` folder.
   - Paste your Discord webhook URL into this file and save it.

   **Example:**
   https://discord.com/api/webhooks/YOUR_WEBHOOK_ID/YOUR_WEBHOOK_TOKEN


5. **Run the game**:
- When you start the game, the plugin will automatically send a notification to your Discord webhook indicating that the game has started.
- Once you join a Photon room, updates about the room will also be sent to the same Discord channel.

## Usage

The plugin provides real-time updates in your Discord channel with the following information:
- **Game Start**: A message is sent when the game starts.
- **Photon Room Info**: When you join a Photon room, a message is sent with the room code, the number of players in the room, and the list of player names.

Example message sent to your Discord webhook:

![image](https://github.com/user-attachments/assets/d96ef499-cc42-4bee-9277-ec510fcc9921)


## Troubleshooting
1. **Missing info.txt**: Ensure that the `info.txt` file is located in the `BepInEx/plugins/SelfTracker/` folder and contains your Discord webhook URL.
2. **No updates in Discord**: Double-check your webhook URL in `info.txt` and verify that the game is using Photon networking.
