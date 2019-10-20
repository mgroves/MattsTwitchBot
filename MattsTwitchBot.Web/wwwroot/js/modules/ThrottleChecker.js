export class ThrottleChecker {
    constructor(localStorage) {
        this.storage = localStorage;
    }

    isThrottled(throttleKey) {
        var throttleTimestamp = this.storage.getItem(throttleKey);
        var threshHold = 300000; // five minutes
        if (throttleTimestamp) {
            var diff = Date.now() - throttleTimestamp;
            if (diff < threshHold) {
                return true;
            }
        }
        this.storage.setItem(throttleKey, Date.now());
        return false;
    }
}
