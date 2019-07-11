"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();
var testing = "";
connection.on("ReceiveCard", function (card, leftOrRight, opponentCard) {
    document.getElementById(leftOrRight).innerHTML = card;
    document.getElementById(opponentCard).remove();
});

connection.on("ReceiveGame", function (game) {
    testing = game + " " + testing;
    document.getElementById("test").innerHTML = testing;
});

function allowDrop(ev) {
    ev.preventDefault();
}

function drag(ev) {
    ev.dataTransfer.setData("text", ev.target.id);
}

function drop(ev) {
    ev.preventDefault();
    var data = ev.dataTransfer.getData("text");
    var opponentCard = data.replace("player", "opponent");
    ev.target.innerHTML = document.getElementById(data).innerHTML;
    connection.invoke("SendCard", document.getElementById(data).innerHTML, ev.target.id, opponentCard).catch(function (err) {
        return console.error(err.toString());
    });
    document.getElementById(data).remove();
}

function testClick() {
    connection.invoke("SendGame", "test").catch(function (err) {
        return console.error(err.toString());
    });
}

connection.start().then(function () {
    document.getElementById("playerCard1").draggable = true;
    document.getElementById("playerCard2").draggable = true;
    document.getElementById("playerCard3").draggable = true;
    document.getElementById("playerCard4").draggable = true;
    document.getElementById("playerCard5").draggable = true;
}).catch(function (err) {
    return console.error(err.toString());
});

/*document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});*/