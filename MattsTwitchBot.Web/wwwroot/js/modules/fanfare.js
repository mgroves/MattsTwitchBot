export class FanfareHandler {
    constructor(videoElement) {
        this.videoElement = videoElement;
    }

    HandleFanfare(fanfareInfo) {
        var video = this.videoElement;

        var url = "https://www.youtube.com/embed/" + fanfareInfo.youTubeCode + "?controls=0&start=" + fanfareInfo.youTubeStartTime + "&autoplay=1&end=" + fanfareInfo.youTubeEndTime + "&modestbranding=1";
        console.log(url);
        if (fanfareInfo) {
            video.src = url;
            setTimeout(function() {
                    video.src = "";
                },
                fanfareInfo.timeout);
        }
        var myNotus = notus();
        myNotus.send({
            notusType: 'toast',
            notusPosition: 'bottom',
            title: 'HYPE HYPE HYPE',
            autoCloseDuration: fanfareInfo.timeout,
            message: fanfareInfo.message,
            animationType: 'slide'
        });
    }
}