///////////////////////////////////////////////////////////////////////////////////////////////////////
//
// Welcome to your first Cloud Script revision!
//
// Cloud Script runs in the PlayFab cloud and has full access to the PlayFab Game Server API 
// (https://api.playfab.com/Documentation/Server), and it runs in the context of a securely
// authenticated player, so you can use it to implement logic for your game that is safe from
// client-side exploits. 
//
// Cloud Script functions can also make web requests to external HTTP
// endpoints, such as a database or private API for your title, which makes them a flexible
// way to integrate with your existing backend systems.
//
// There are several different options for calling Cloud Script functions:
//
// 1) Your game client calls them directly using the "ExecuteCloudScript" API,
// passing in the function name and arguments in the request and receiving the 
// function return result in the response.
// (https://api.playfab.com/Documentation/Client/method/ExecuteCloudScript)
// 
// 2) You create PlayStream event actions that call them when a particular 
// event occurs, passing in the event and associated player profile data.
// (https://api.playfab.com/playstream/docs)
// 
// 3) For titles using the Photon Add-on (https://playfab.com/marketplace/photon/),
// Photon room events trigger webhooks which call corresponding Cloud Script functions.
// 
// The following examples demonstrate all three options.
//
///////////////////////////////////////////////////////////////////////////////////////////////////////

///////////////////////////////////////////////////////////////////////////////////////////////////////

handlers.startActionGame = function (args, context) {
    
    var player_data = server.GetUserInternalData({
        PlayFabId: currentPlayerId,
        Keys: ["level"]
    });
    
    var player_level = player_data.Data.level ? parseInt(player_data.Data.level.Value) :1;
    
    if(player_level < args.currentPlayLevel){
        return {
            status: "CannotPlayLevel",
            message: "You cannot play that level"};
    }
    
    // log.info(args.currentCharacter);
    
    var player_character = server.GetCharacterData({
        PlayFabId: currentPlayerId,
        CharacterId: args.currentCharacter
    });
    
    if(!player_character.CharacterId){
        return {
            status: "DonthaveCharacter",
            message: "You dont have this character"};
    }
    
    var updateUserDataResult = server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            level: player_level,
            currentCharacter: args.currentCharacter,
            currentPlayLevel: args.currentPlayLevel,
            currentPlayStartTime: Date.now(),
            currentStatus: "Start"
        }
    });

    return {
        status: "Success",
        message: "update successful"};
};

handlers.completeActionGame = function (args, context){
    
    var player_data = server.GetUserInternalData({
        PlayFabId: currentPlayerId,
        Keys: ["currentPlayLevel","currentPlayStartTime","level","currentCharacter","currentStatus"]
    });
    
    // log.info(args.currentPlayLevel);
    // log.info(player_data.Data.currentPlayLevel);
    
    if(args.currentPlayLevel!=parseInt(player_data.Data.currentPlayLevel.Value)){
        return {
            status: "CheatDetected",
            message: "Please do not cheat"};
    }
    if(player_data.Data.currentStatus.Value=="Complete"){
        return {
            status: "AlreadyComplete",
            message: "The Game is already completed"
        };
    }
    
    var title_data = server.GetTitleData({
       Keys: ["default_level_time_limit","default_man_max_dmg_rate","powerful_man_max_dmg_rate","power_up_max_dmg_rate_mul","default_score_to_point_rate"] 
    });
    
    var time_taken = diff_seconds(Date.now(),parseInt(player_data.Data.currentPlayStartTime.Value));
    log.info(time_taken);
    // log.info(title_data);
    if(time_taken > parseInt(title_data.Data.default_level_time_limit) ){
        return {
            status: "GameTimeOut",
            message: "time is out"};
    }
    
    var max_possible_score_rate = 0;
    
    var player_characters = server.GetAllUsersCharacters({
        PlayFabId: currentPlayerId
    });
    
    var current_character_type = "";
    // log.info(player_characters);
    // log.info(player_data.Data.currentCharacter.Value);
    for (var c =0; c < player_characters.Characters.length; c++){
        // log.info(c);
        // log.info(player_characters.Characters[c]);
        // log.info(player_characters.Characters[c].CharacterId);
        // log.info(player_characters.Characters[c].CharacterId == player_data.Data.currentCharacter.Value);
        if(player_characters.Characters[c].CharacterId == player_data.Data.currentCharacter.Value){
            current_character_type = player_characters.Characters[c].CharacterType;
            log.info("old current char type:"+current_character_type);
        }
    }
    log.info("current char type:"+current_character_type);
    switch(current_character_type){
        case "default_man01":
            max_possible_score_rate = parseInt(title_data.Data.default_man_max_dmg_rate);
            break;
        case "powerful_man01":
            max_possible_score_rate = parseInt(title_data.Data.powerful_man_max_dmg_rate);
            break;
    }
    log.info(args.usedItems);
    // log.info(args.usedItems.includes("attack_up"));
    if(args.usedItems.includes("attack_up")){
        max_possible_score_rate *= parseInt(title_data.Data.power_up_max_dmg_rate_mul);
    }
    log.info(time_taken * max_possible_score_rate);
    
    if(args.score > time_taken * max_possible_score_rate){
        return {
            status: "CheatDetected",
            message: "Stop cheating, that is impossible!"};
    }
    
    //if the form of usedItem is like this:
    //{attack_up_instance_id:0,defense_up_instance_id:0,speed_up_instance_id:1,health_potion_instance_id:0}
    //then uncomment the following line
    
    // for(var key in args.usedItems){
    //     server.ModifyItemUses({
    //       PlayFabId: currentPlayerId,
    //       ItemInstanceId: key,
    //       UsesToAdd: -args.usedItems[key]
    //     });
    // }
    
    server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            currentStatus: "Complete"
        }
    });
    server.UpdatePlayerStatistics({
        PlayFabId: currentPlayerId, Statistics: [{
            StatisticName: "DailyScore",
            Value: args.score
        },{
            StatisticName: "WeeklyScore",
            Value: args.score
        }]
    });
    
    var gained_points = Math.round(args.score * parseFloat(title_data.Data.default_score_to_point_rate));
    
    server.AddUserVirtualCurrency({
        PlayFabId: currentPlayerId,
        VirtualCurrency: "FP",
        Amount: gained_points
    });
    
    if(args.currentPlayLevel == parseInt(player_data.Data.level.Value)){
        server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            level : parseInt(player_data.Data.level.Value) + 1
        }
    });
    }
    
    return {
        status: "Success",
        message: "update successful"};
    
};

handlers.convertFPToGD = function (args, context){
    var player_inventory = server.GetUserInventory({
        PlayFabId: currentPlayerId
    });
    
    var title_data = server.GetTitleData({
       Keys: ["default_point_to_currency_rate"] 
    });
    
    log.info(player_inventory.VirtualCurrency);
    log.info(player_inventory.VirtualCurrency.FP);
    log.info(args.FightingPoint > player_inventory.VirtualCurrency.FP);
    if(args.FightingPoint > player_inventory.VirtualCurrency.FP){
        return {
            status: "NotEnoughFP",
            message: "not enough FP"};
    }
    
    var gained_gold = args.FightingPoint * parseInt(title_data.Data.default_point_to_currency_rate);
    log.info(gained_gold);
    
    server.SubtractUserVirtualCurrency({
        PlayFabId: currentPlayerId,
        VirtualCurrency: "FP",
        Amount: args.FightingPoint
    });
    
    server.AddUserVirtualCurrency({
        PlayFabId: currentPlayerId,
        VirtualCurrency: "GD",
        Amount: gained_gold
    });
    
    return {
        status: "Success",
        message: "update successful"};
};

function diff_seconds(dt2, dt1) 
 {

  var diff =(dt2 - dt1) / 1000;
  return Math.abs(Math.round(diff));
  
 };
///////////////////////////////////////////////////////////////////////////////////////////////////////

// This is a Cloud Script function. "args" is set to the value of the "FunctionParameter" 
// parameter of the ExecuteCloudScript API.
// (https://api.playfab.com/Documentation/Client/method/ExecuteCloudScript)
// "context" contains additional information when the Cloud Script function is called from a PlayStream action.
handlers.helloWorld = function (args, context) {
    
    // The pre-defined "currentPlayerId" variable is initialized to the PlayFab ID of the player logged-in on the game client. 
    // Cloud Script handles authenticating the player automatically.
    var message = "Hello " + currentPlayerId + "!";

    // You can use the "log" object to write out debugging statements. It has
    // three functions corresponding to logging level: debug, info, and error. These functions
    // take a message string and an optional object.
    log.info(message);
    var inputValue = null;
    if (args && args.inputValue)
        inputValue = args.inputValue;
    log.debug("helloWorld:", { input: args.inputValue });

    // The value you return from a Cloud Script function is passed back 
    // to the game client in the ExecuteCloudScript API response, along with any log statements
    // and additional diagnostic information, such as any errors returned by API calls or external HTTP
    // requests. They are also included in the optional player_executed_cloudscript PlayStream event 
    // generated by the function execution.
    // (https://api.playfab.com/playstream/docs/PlayStreamEventModels/player/player_executed_cloudscript)
    return { messageValue: message };
};

// This is a simple example of making a PlayFab server API call
handlers.makeAPICall = function (args, context) {
    var request = {
        PlayFabId: currentPlayerId, Statistics: [{
                StatisticName: "Level",
                Value: 2
            }]
    };
    // The pre-defined "server" object has functions corresponding to each PlayFab server API 
    // (https://api.playfab.com/Documentation/Server). It is automatically 
    // authenticated as your title and handles all communication with 
    // the PlayFab API, so you don't have to write extra code to issue HTTP requests. 
    var playerStatResult = server.UpdatePlayerStatistics(request);
};

// This an example of a function that calls a PlayFab Entity API. The function is called using the 
// 'ExecuteEntityCloudScript' API (https://api.playfab.com/documentation/CloudScript/method/ExecuteEntityCloudScript).
handlers.makeEntityAPICall = function (args, context) {

    // The profile of the entity specified in the 'ExecuteEntityCloudScript' request.
    // Defaults to the authenticated entity in the X-EntityToken header.
    var entityProfile = context.currentEntity;

    // The pre-defined 'entity' object has functions corresponding to each PlayFab Entity API,
    // including 'SetObjects' (https://api.playfab.com/documentation/Data/method/SetObjects).
    var apiResult = entity.SetObjects({
        Entity: entityProfile.Entity,
        Objects: [
            {
                ObjectName: "obj1",
                DataObject: {
                    foo: "some server computed value",
                    prop1: args.prop1
                }
            }
        ]
    });

    return {
        profile: entityProfile,
        setResult: apiResult.SetResults[0].SetResult
    };
};

// This is a simple example of making a web request to an external HTTP API.
handlers.makeHTTPRequest = function (args, context) {
    var headers = {
        "X-MyCustomHeader": "Some Value"
    };
    
    var body = {
        input: args,
        userId: currentPlayerId,
        mode: "foobar"
    };

    var url = "http://httpbin.org/status/200";
    var content = JSON.stringify(body);
    var httpMethod = "post";
    var contentType = "application/json";

    // The pre-defined http object makes synchronous HTTP requests
    var response = http.request(url, httpMethod, content, contentType, headers);
    return { responseContent: response };
};

// This is a simple example of a function that is called from a
// PlayStream event action. (https://playfab.com/introducing-playstream/)
handlers.handlePlayStreamEventAndProfile = function (args, context) {
    
    // The event that triggered the action 
    // (https://api.playfab.com/playstream/docs/PlayStreamEventModels)
    var psEvent = context.playStreamEvent;
    
    // The profile data of the player associated with the event
    // (https://api.playfab.com/playstream/docs/PlayStreamProfileModels)
    var profile = context.playerProfile;
    
    // Post data about the event to an external API
    var content = JSON.stringify({ user: profile.PlayerId, event: psEvent.EventName });
    var response = http.request('https://httpbin.org/status/200', 'post', content, 'application/json', null);

    return { externalAPIResponse: response };
};


// Below are some examples of using Cloud Script in slightly more realistic scenarios

// This is a function that the game client would call whenever a player completes
// a level. It updates a setting in the player's data that only game server
// code can write - it is read-only on the client - and it updates a player
// statistic that can be used for leaderboards. 
//
// A funtion like this could be extended to perform validation on the 
// level completion data to detect cheating. It could also do things like 
// award the player items from the game catalog based on their performance.
handlers.completedLevel = function (args, context) {
    var level = args.levelName;
    var monstersKilled = args.monstersKilled;
    
    var updateUserDataResult = server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            lastLevelCompleted: level
        }
    });

    log.debug("Set lastLevelCompleted for player " + currentPlayerId + " to " + level);
    var request = {
        PlayFabId: currentPlayerId, Statistics: [{
                StatisticName: "level_monster_kills",
                Value: monstersKilled
            }]
    };
    server.UpdatePlayerStatistics(request);
    log.debug("Updated level_monster_kills stat for player " + currentPlayerId + " to " + monstersKilled);
};


// In addition to the Cloud Script handlers, you can define your own functions and call them from your handlers. 
// This makes it possible to share code between multiple handlers and to improve code organization.
handlers.updatePlayerMove = function (args) {
    var validMove = processPlayerMove(args);
    return { validMove: validMove };
};


// This is a helper function that verifies that the player's move wasn't made
// too quickly following their previous move, according to the rules of the game.
// If the move is valid, then it updates the player's statistics and profile data.
// This function is called from the "UpdatePlayerMove" handler above and also is 
// triggered by the "RoomEventRaised" Photon room event in the Webhook handler
// below. 
//
// For this example, the script defines the cooldown period (playerMoveCooldownInSeconds)
// as 15 seconds. A recommended approach for values like this would be to create them in Title
// Data, so that they can be queries in the script with a call to GetTitleData
// (https://api.playfab.com/Documentation/Server/method/GetTitleData). This would allow you to
// make adjustments to these values over time, without having to edit, test, and roll out an
// updated script.
function processPlayerMove(playerMove) {
    var now = Date.now();
    var playerMoveCooldownInSeconds = 15;

    var playerData = server.GetUserInternalData({
        PlayFabId: currentPlayerId,
        Keys: ["last_move_timestamp"]
    });

    var lastMoveTimestampSetting = playerData.Data["last_move_timestamp"];

    if (lastMoveTimestampSetting) {
        var lastMoveTime = Date.parse(lastMoveTimestampSetting.Value);
        var timeSinceLastMoveInSeconds = (now - lastMoveTime) / 1000;
        log.debug("lastMoveTime: " + lastMoveTime + " now: " + now + " timeSinceLastMoveInSeconds: " + timeSinceLastMoveInSeconds);

        if (timeSinceLastMoveInSeconds < playerMoveCooldownInSeconds) {
            log.error("Invalid move - time since last move: " + timeSinceLastMoveInSeconds + "s less than minimum of " + playerMoveCooldownInSeconds + "s.");
            return false;
        }
    }

    var playerStats = server.GetPlayerStatistics({
        PlayFabId: currentPlayerId
    }).Statistics;
    var movesMade = 0;
    for (var i = 0; i < playerStats.length; i++)
        if (playerStats[i].StatisticName === "")
            movesMade = playerStats[i].Value;
    movesMade += 1;
    var request = {
        PlayFabId: currentPlayerId, Statistics: [{
                StatisticName: "movesMade",
                Value: movesMade
            }]
    };
    server.UpdatePlayerStatistics(request);
    server.UpdateUserInternalData({
        PlayFabId: currentPlayerId,
        Data: {
            last_move_timestamp: new Date(now).toUTCString(),
            last_move: JSON.stringify(playerMove)
        }
    });

    return true;
}

// This is an example of using PlayStream real-time segmentation to trigger
// game logic based on player behavior. (https://playfab.com/introducing-playstream/)
// The function is called when a player_statistic_changed PlayStream event causes a player 
// to enter a segment defined for high skill players. It sets a key value in
// the player's internal data which unlocks some new content for the player.
handlers.unlockHighSkillContent = function (args, context) {
    var playerStatUpdatedEvent = context.playStreamEvent;
    var request = {
        PlayFabId: currentPlayerId,
        Data: {
            "HighSkillContent": "true",
            "XPAtHighSkillUnlock": playerStatUpdatedEvent.StatisticValue.toString()
        }
    };
    var playerInternalData = server.UpdateUserInternalData(request);
    log.info('Unlocked HighSkillContent for ' + context.playerProfile.DisplayName);
    return { profile: context.playerProfile };
};

// Photon Webhooks Integration
//
// The following functions are examples of Photon Cloud Webhook handlers. 
// When you enable the Photon Add-on (https://playfab.com/marketplace/photon/)
// in the Game Manager, your Photon applications are automatically configured
// to authenticate players using their PlayFab accounts and to fire events that 
// trigger your Cloud Script Webhook handlers, if defined. 
// This makes it easier than ever to incorporate multiplayer server logic into your game.


// Triggered automatically when a Photon room is first created
handlers.RoomCreated = function (args) {
    log.debug("Room Created - Game: " + args.GameId + " MaxPlayers: " + args.CreateOptions.MaxPlayers);
};

// Triggered automatically when a player joins a Photon room
handlers.RoomJoined = function (args) {
    log.debug("Room Joined - Game: " + args.GameId + " PlayFabId: " + args.UserId);
};

// Triggered automatically when a player leaves a Photon room
handlers.RoomLeft = function (args) {
    log.debug("Room Left - Game: " + args.GameId + " PlayFabId: " + args.UserId);
};

// Triggered automatically when a Photon room closes
// Note: currentPlayerId is undefined in this function
handlers.RoomClosed = function (args) {
    log.debug("Room Closed - Game: " + args.GameId);
};

// Triggered automatically when a Photon room game property is updated.
// Note: currentPlayerId is undefined in this function
handlers.RoomPropertyUpdated = function (args) {
    log.debug("Room Property Updated - Game: " + args.GameId);
};

// Triggered by calling "OpRaiseEvent" on the Photon client. The "args.Data" property is 
// set to the value of the "customEventContent" HashTable parameter, so you can use
// it to pass in arbitrary data.
handlers.RoomEventRaised = function (args) {
    var eventData = args.Data;
    log.debug("Event Raised - Game: " + args.GameId + " Event Type: " + eventData.eventType);

    switch (eventData.eventType) {
        case "playerMove":
            processPlayerMove(eventData);
            break;

        default:
            break;
    }
};
