export function SocialMediaFlyIn() {
    var secondsToDisplay = 6000;
    var secondsBetween = 500;
    var badges = $(".socialMediaBadge");
    for (var i = 0; i < badges.length; i++) {
        var el = $(".socialMediaBadge")[i];
        $(el)
            .delay(i * (secondsToDisplay + secondsBetween))
            .animate({ right: '0px' })
            .delay(secondsToDisplay)
            .animate({ right: '-500px' })
            .delay(secondsBetween);
    }
}