export function Ticker() {
    // TODO: pick a ticker
    var tickers = $(".ticker");
    var ticker = $(tickers[Math.floor(Math.random() * tickers.length)]);

    // TODO: make it visible
    $(ticker).css("display", "block");

    // TODO: hide the ticker
    setTimeout(function() {
        $(ticker).css("display", "none");
    }, 30000);
}