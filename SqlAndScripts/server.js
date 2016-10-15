var userId;

var http = require('http');

var client = require("socket.io").listen(5555).sockets;

client.on("connection", function (socket) {

    var handshakeData = socket.request;

    userId = handshakeData._query['userid'];

    console.log("ConnectedUserId: " + userId);

    if (userId != null && userId != "" && userId != undefined) {
        CheckOldFeeds(function (e) {

            socket.emit("oldfeeds", e);

        }, userId);
    }

    socket.on("push", function (data) {

        console.log("Message Data Received" + data);

        PostFeed(function (e) {

            if (e.length > 0) {
                for (var i = 0; i < e.length; i++) {

                    client.emit("message", e[i]);

                    UpdateFeedAsPosted(e[i].Id);
                }
            }

        }, data);

        //client.emit("message", data);

    });
});

function CheckOldFeeds(callBackFunction, userId) {
    var jData = {
        userId: userId
    };


    var jSonData = JSON.stringify(jData);

    var headers = {
        'Content-Type': 'application/json',
        'Content-Length': jSonData.length
    };

    var options = {
        host: '172.19.1.66',
        port: 8080,
        path: '/WebServices/NEF.WebServices.SalesPortal/SalesPortal.svc/GetOldFeeds',
        method: 'POST',
        headers: headers
    };

    var req = http.request(options, function (res) {
        res.setEncoding('utf-8');

        var responseString = '';

        res.on('data', function (data) {
            responseString += data;
        });

        res.on('end', function () {

            var resultObject = JSON.parse(responseString);
            //console.log(resultObject[0].Name);
            callBackFunction(resultObject);

            console.log(resultObject.length);
        });
    });

    req.on('error', function (e) {
        console.log("HATA");
    });

    req.write(jSonData);
    req.end();
}

function PostFeed(callBackFunction, comingData) {

    var jData = {};

    var jSonData = JSON.stringify(jData);

    var headers = {
        'Content-Type': 'application/json',
        'Content-Length': jSonData.length
    };

    var options = {
        host: '172.19.1.66',
        port: 8080,
        path: '/WebServices/NEF.WebServices.SalesPortal/SalesPortal.svc/GetNotPostedFeeds',
        method: 'POST',
        headers: headers
    };

    var req = http.request(options, function (res) {
        res.setEncoding('utf-8');

        var responseString = '';

        res.on('data', function (data) {
            responseString += data;
        });

        res.on('end', function () {
            var resultObject = JSON.parse(responseString);

            callBackFunction(resultObject);

            console.log("Not Posted Feeds:" + resultObject.length);
        });
    });

    req.on('error', function (e) {
        console.log("HATA");
    });

    req.write(jSonData);
    req.end();
}

function UpdateFeedAsPosted(feedId) {

    var jData = {};
    jData.feedId = feedId;

    var jSonData = JSON.stringify(jData);

    var headers = {
        'Content-Type': 'application/json',
        'Content-Length': jSonData.length
    };

    var options = {
        host: '172.19.1.66',
        port: 8080,
        path: '/WebServices/NEF.WebServices.SalesPortal/SalesPortal.svc/UpdateFeedAsPosted',
        method: 'POST',
        headers: headers
    };

    var req = http.request(options, function (res) {
        res.setEncoding('utf-8');

        var responseString = '';

        res.on('data', function (data) {
            responseString += data;
        });

        res.on('end', function () {
            var resultObject = JSON.parse(responseString);

            console.log("Feed Updated As Posted:" + feedId);
        });
    });

    req.on('error', function (e) {
        console.log("HATA");
    });

    req.write(jSonData);
    req.end();
}