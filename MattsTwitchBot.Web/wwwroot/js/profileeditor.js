import { FanfareHandler } from './modules/fanfare.js';

function GetFanfareInfoFromForm() {
    var fanfareInfo = {};
    fanfareInfo.youTubeCode = document.getElementById("FanfareYouTubeCode").value;
    fanfareInfo.timeout = document.getElementById("FanfareTimeout").value;
    fanfareInfo.message = document.getElementById("FanfareMessage").value;
    fanfareInfo.youTubeStartTime = document.getElementById("FanfareYouTubeStartTime").value;
    fanfareInfo.youTubeEndTime = document.getElementById("FanfareYouTubeEndTime").value;
    console.log(fanfareInfo);
    return fanfareInfo;
}

var btnFanfarePreview = document.getElementById("btnFanfarePreview");
btnFanfarePreview.addEventListener("click", function () {
    var fanfare = new FanfareHandler(document.getElementById("ytvideo"));

    var fanfareTestInfo = GetFanfareInfoFromForm();

    fanfare.HandleFanfare(fanfareTestInfo);
});