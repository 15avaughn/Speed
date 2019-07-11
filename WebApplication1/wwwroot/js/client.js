"use strict";
$("#register").show();
$("#findOpponent").hide();
$("#findingOpponent").hide();
$("#game").hide();
var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();
var testing = "";
var playerName = "";
connection.on("ReceiveCard", function (card, leftOrRight, opponentCard) {
    document.getElementById(leftOrRight).innerHTML = card;
    document.getElementById(opponentCard).remove();
});

connection.on("test", function () {
    $("#test").html("<br/><span><strong>Something went wrong.</strong></span>");
});

connection.on("ReceiveGame", function (game) {
    testing = game + " " + testing;
    document.getElementById("test").innerHTML = testing;
});

connection.on('registrationComplete', data => {
    $("#register").hide();
    $("#findOpponent").show();
});

connection.on('opponentFound', (data, image) => {
    $('#findOpponent').hide();
    $('#findingOpponent').hide();
    $('#game').show();
    $('#test').html("<br/><span><strong> Hey " + playerName + "! You are playing against <i>" + data + "</i></strong></span>");
    
});

connection.on('opponentNotFound', data => {
    $('#findOpponent').hide();
    $('#findingOpponent').show();
});

connection.on('opponentDisconnected', data => {
    $("#register").hide();
    $('#game').hide();
    $('#test').html("<br/><span><strong>Hey " + playerName + "! Your opponent disconnected or left the battle! You are the winner ! Hip Hip Hurray!!!</strong></span>");

});

$("#btnRegister").click(function () {
    playerName = $('#name').val();
    connection.invoke('RegisterPlayer', playerName);
});

$("#btnFindOpponentPlayer").click(function () {
    connection.invoke('FindOpponent');
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