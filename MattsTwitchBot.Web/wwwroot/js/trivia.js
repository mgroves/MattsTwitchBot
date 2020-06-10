window.onload = async function () {

    // load question data from endpoint
    // TODO: right now this is the only ajax/fetch endpoint
    //      if you add 1 or 2 more, you should wrap this bearer token + fetch stuff into its own function
    var bearerToken = $.cookie("bearertoken");
    var resp = await fetch("/gettriviadata?token=" + bearerToken);
    var respData = await resp.json();
    var questionData = respData.questions;
    var messages = respData.messages;
    var totalNumMessages = 0;
    var totalNumTrivia = questionData.length;
    if (messages)
        totalNumMessages = messages.length;
    var messageToShow = 0;
    var insertMessageEvery = Math.floor(totalNumTrivia / totalNumMessages);

    for (var i = 0; i < questionData.length; i++) {

        var obj = questionData[i];

        // render the question and answers
        obj.autoSlideTime = 20000;
        var questionTemplate = document.getElementById('questionTemplate').innerHTML;
        var rendered = Mustache.render(questionTemplate, obj);

        // render the question and answers with the answer highlighted
        for (var j = 0; j < obj.options.length; j++) {
            if (j === obj.answer) {
                obj.options[j] = "<div class=\"trivia-answer\">" + obj.options[j] + "</div>";
            }
        }

        obj.autoSlideTime = 10000;
        var renderedAgain = Mustache.render(questionTemplate, obj);
        $("#slide-intro").after(renderedAgain);
        $("#slide-intro").after(rendered);
        
        var shouldInsertMessageNow = ((i % insertMessageEvery) === 0) && (totalNumMessages !== messageToShow);
        if (shouldInsertMessageNow) {
            var messageTemplate = document.getElementById('messageTemplate').innerHTML;
            var renderedMessage = Mustache.render(messageTemplate, { message: messages[messageToShow] });
            $("#slide-intro").after(renderedMessage);
            messageToShow++;
        }
    }

    Reveal.configure({
        autoSlide: 60000
    });
    Reveal.initialize();
};