import { ThrottleChecker } from '../modules/ThrottleChecker.js';

describe("ThrottleChecker tests", function () {

    it("is not throttled if hasn't been set already", function () {
        // arrange
        var fakeStorage = {
            storageStr: null,
            storageVal: null,
            getItem: function (str) {
                return null;
            },
            setItem: function(str, val) {
                this.storageStr = str;
                this.storageVal = val;
            }
        };

        var throttle = new ThrottleChecker(fakeStorage);

        // act
        var result = throttle.isThrottled("doesntmatter");

        // assert
        expect(result).toBe(false);
    });
});
