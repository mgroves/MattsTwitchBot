var connection = new signalR.HubConnectionBuilder().withUrl("/twitchHub").build();

connection.on("SoundEffect",
    function (soundEffectName) {
        // TODO: throttling here or throttling server side?
        var filename = "";
        if (soundEffectName === "laugh") {
            filename = "media/laugh.mp3"; // https://freesound.org/people/FunWithSound/sounds/381374/
        }
        if (soundEffectName === "rimshot") {
            filename = "media/rimshot.wav"; // https://freesound.org/people/xtrgamr/sounds/432972/
        }
        if (filename) {
            var audio = new Audio(filename);    // https://stackoverflow.com/questions/9419263/playing-audio-with-javascript
            audio.play();
        }
    }
);

connection.start().then(function () {
    //
}).catch(function (err) {
    return console.error(err.toString());
});
