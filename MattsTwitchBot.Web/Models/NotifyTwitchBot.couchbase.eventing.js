/*
 This is meant to be a function that's deployed to the Couchbase cluster.
 It will cause Couchbase to call back to the given HTTP endpoint with the document
 that has been changed.

You can then choose how to handle (or ignore) the document change within ASP.NET Core.

So it's basically a Rube-Goldbergian way to "subscribe" to document changes in Couchbase from ASP.NET
until such time as the real thing is added to Couchbase Server.

This may not be a great idea in production, especially for large numbers of documents and/or large numbers of changes
just because of the sheer amount of HTTP traffic it will create. But who knows, it might be totally fine.
Defintely make sure whatever endpoint you use is async!

As an example, to create the image for this chatbot, create a function with these settings:

Source Bucket: twitchchat
Metadata Bucket: eventingmetadata
Function Name: NotifyTwitchBot
Description: Whatever
Binding:
    URL alias, boturl, http://mattstwitchbot.web/couchbasenotify

Then paste the below JavaScript in. If you only wanted to "subscribe" to certain kinds of document changes,
then you can add 'if' statements.

For instance, if you wanted to exclude documents of type "foo", add this at the beginning:
    if (doc.type != "foo")
        return;
 */

function OnUpdate(doc, meta) {
    var request = {
        'body' : doc
    };
    curl('POST', boturl, request);
}
function OnDelete(meta) {
}