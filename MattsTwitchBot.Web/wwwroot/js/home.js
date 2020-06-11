import { SocialMediaFlyIn } from './modules/SocialMediaFlyIn.js';
import { Ticker } from './modules/Ticker.js';

window.testFlyin = function () {
    SocialMediaFlyIn();
}

window.testTicker = function() {
    Ticker();
}

function loopSocialMedia() {
    setTimeout(function() {
        SocialMediaFlyIn();
        loopSocialMedia();
    }, 600000);
};

function loopTicker() {
    setTimeout(function () {
        Ticker();
        loopTicker();
    }, 700000);
}

SocialMediaFlyIn();
loopSocialMedia();
loopTicker();