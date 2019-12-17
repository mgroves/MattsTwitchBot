import { ThrottleChecker } from './modules/ThrottleChecker.js';
import { FanfareHandler } from './modules/fanfare.js';

window.testFanfare = function (username) {
    HandleFanfare({ "youTubeCode": "NkVQnZ3xndI", "timeout": "6000", "message": "Calvin!", "youTubeStartTime": "217", "youTubeEndTime": "220" });
};

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

function HandleFanfare(fanfareInfo) {
    console.log(fanfareInfo);
    var fanfare = new FanfareHandler(document.getElementById("ytvideo"));
    fanfare.HandleFanfare(fanfareInfo);
}

function HandleSoundEffect(soundEffectName) {
    var throttle = new ThrottleChecker(localStorage);

    var filename = GetSoundEffectFileName(soundEffectName);
    if (!filename)
        return;

    if (!throttle.isThrottled(soundEffectName + "_throttle")) {
        var audio = new window.Audio(filename);    // https://stackoverflow.com/questions/9419263/playing-audio-with-javascript
        audio.play();
    }
}

function GetSoundEffectFileName(soundEffectName) {
    if (soundEffectName === "laugh") {
        return "media/laugh.mp3"; // https://freesound.org/people/FunWithSound/sounds/381374/
    }
    if (soundEffectName === "rimshot" || soundEffectName === "badumtss") {
        return "media/rimshot.wav"; // https://freesound.org/people/xtrgamr/sounds/432972/
    }
    if (soundEffectName === "sadtrombone") {
        return "media/sadtrombone.mp3"; // https://freesound.org/people/NotR/sounds/172949/
    }
    return "";
}
