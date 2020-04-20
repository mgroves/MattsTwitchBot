import { SocialMediaFlyIn } from './modules/SocialMediaFlyIn.js'

window.testFlyin = function () {
    SocialMediaFlyIn();
}

function loop() {
    setTimeout(function() {
        SocialMediaFlyIn();
        loop();
    }, 600000);
};

SocialMediaFlyIn();
loop();