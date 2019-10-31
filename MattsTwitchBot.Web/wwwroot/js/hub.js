import { ThrottleChecker } from './modules/ThrottleChecker.js';

window.testFanfare = function (username) {
    if (!username)
        username = "matthewdgroves";
    HandleFanfare(username)
};

window.onload = function () {
    var connection = new signalR.HubConnectionBuilder().withUrl("/twitchHub").build();
    connection.on("ReceiveSoundEffect", HandleSoundEffect);
    connection.on("ReceiveFanfare", HandleFanfare);
    connection.start().then(function () {
        console.log("Hello, Twitch. My Chat Bot is ready!");
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

function HandleFanfare(userName) {
    var video = document.getElementById("ytvideo");
    var fanfareInfo = GetFanfareInfo(userName);
    if (fanfareInfo) {
        video.src = fanfareInfo.url;
        setTimeout(function () {
            video.src = "";
        }, fanfareInfo.timeout);
    }
    var myNotus = notus();
    myNotus.send({
        notusType: 'toast',
        notusPosition: 'bottom',
        title: 'HYPE HYPE HYPE',
        autoCloseDuration: fanfareInfo.timeout,
        message: 'Welcome to the channel, ' + userName + "!",
        animationType: 'slide'
    });
}

function HandleSoundEffect(soundEffectName) {
    console.log("HandleSoundEffect");
    var throttle = new ThrottleChecker(localStorage);

    var filename = GetSoundEffectFileName(soundEffectName);
    if (!filename)
        return;

    if (!throttle.isThrottled(soundEffectName + "_throttle")) {
        var audio = new window.Audio(filename);    // https://stackoverflow.com/questions/9419263/playing-audio-with-javascript
        audio.play();
    }
}

function GetFanfareInfo(userName) {
    if (userName === "calvinaallen") {
        return {
            timeout: 5000,
            url: "https://www.youtube.com/embed/NkVQnZ3xndI?controls=0&start=217&autoplay=1&end=220&modestbranding=1"
        };
    }
    if (userName === "matthewdgroves") {
        return {
            timeout: 16000,
            url: "https://www.youtube.com/embed/DGsBRImD0po?controls=0&start=94&autoplay=1&end=109&modestbranding=1"
        };
    }

    return {};
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
