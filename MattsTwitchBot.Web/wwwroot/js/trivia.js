window.onload = async function () {

    // load question data from endpoint
    // TODO: right now this is the only ajax/fetch endpoint
    //      if you add 1 or 2, you should wrap this bearer token + fetch stuff into its own function
    debugger;
    var bearerToken = $.cookie("bearertoken");
    var resp = await fetch("/gettriviadata?token=" + bearerToken);
    var respData = await resp.json();
    var questionData = respData.questions;

    for (var i = 0; i < questionData.length; i++) {

        var obj = questionData[i];

        // render the question and answers
        obj.autoSlideTime = 20000;
        var template = document.getElementById('template').innerHTML;
        var rendered = Mustache.render(template, obj);

        // render the question and answers with the answer highlighted
        for (var j = 0; j < obj.options.length; j++) {
            if (j === obj.answer) {
                obj.options[j] = "<div class=\"trivia-answer\">" + obj.options[j] + "</div>";
            }
        }

        obj.autoSlideTime = 10000;
        var renderedAgain = Mustache.render(template, obj);
        $("#slide-intro").after(renderedAgain);
        $("#slide-intro").after(rendered);
    }

    Reveal.configure({
        autoSlide: 60000
    });
    Reveal.initialize();
};