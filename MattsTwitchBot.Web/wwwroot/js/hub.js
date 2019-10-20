import { ThrottleChecker } from './modules/ThrottleChecker.js';

var connection = new signalR.HubConnectionBuilder().withUrl("/twitchHub").build();

connection.on("SoundEffect", HandleSoundEffect);

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
    if (soundEffectName === "rimshot") {
        return "media/rimshot.wav"; // https://freesound.org/people/xtrgamr/sounds/432972/
    }
    if (soundEffectName === "sadtrombone") {
        return "media/sadtrombone.mp3"; // https://freesound.org/people/NotR/sounds/172949/
    }
    return "";
}

connection.start().then(function () {
    //
}).catch(function (err) {
    return console.error(err.toString());
});
