var connection = new signalR.HubConnectionBuilder().withUrl("/twitchHub").build();

connection.on("SoundEffect",
    function (soundEffectName) {
        var filename = GetSoundEffectFileName(soundEffectName);
        if (!filename)
            return;

        if (!IsThrottled(soundEffectName + "_throttle")) {
            var audio = new Audio(filename);    // https://stackoverflow.com/questions/9419263/playing-audio-with-javascript
            audio.play();
        }
    }
);

function IsThrottled(throttleKey) {
    var throttleTimestamp = localStorage.getItem(throttleKey);
    var threshHold = 300000; // five minutes
    if (throttleTimestamp) {
        var diff = Date.now() - throttleTimestamp;
        if (diff < threshHold) {
            return true;
        }
    }
    localStorage.setItem(throttleKey, Date.now());
    return false;
}

function GetSoundEffectFileName(soundEffectName) {
    if (soundEffectName === "laugh") {
        return "media/laugh.mp3"; // https://freesound.org/people/FunWithSound/sounds/381374/
    }
    if (soundEffectName === "rimshot") {
        return "media/rimshot.wav"; // https://freesound.org/people/xtrgamr/sounds/432972/
    }
    return;
}

connection.start().then(function () {
    //
}).catch(function (err) {
    return console.error(err.toString());
});
