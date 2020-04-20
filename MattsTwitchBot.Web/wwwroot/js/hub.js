import { HandleFanfare } from './modules/HandleFanfare.js';
import { HandleSoundEffect } from './modules/HandleSoundEffect.js'

window.testFanfare = function () {
    HandleFanfare({ "youTubeCode": "NkVQnZ3xndI", "timeout": "6000", "message": "Calvin!", "youTubeStartTime": "217", "youTubeEndTime": "220" });
};

window.testSoundEffect = function() {
    HandleSoundEffect("laugh");
}

window.onload = function () {
    var connection = new signalR.HubConnectionBuilder().withUrl("/twitchHub").build();

    connection.on("ReceiveSoundEffect", HandleSoundEffect);
    connection.on("ReceiveFanfare", HandleFanfare);

    connection.start().then(function () {
    console.log("Hello, console. My Chat Bot is ready!");
    var myNotus = notus();
    myNotus.send({
        notusType: 'toast',
        notusPosition: 'bottom',
        title: 'Bot ready',
        autoCloseDuration: 5000,
        message: 'Hello, Twitch. My Chat Bot is ready!',
        animationType: 'slide'
    });
    }).catch(function (err) {
        return console.error(err.toString());
    });
};
