"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/gameHub").build();
var testing = "";
var playerName = "";
var opponentName = "";
var playerReset = false;
var opponentReset = false;
connection.on("ReceiveCard", function (sentCard, centerStack) {
    $("#" + centerStack).attr("src", "/images/" + sentCard + ".svg");
});

connection.on("drawGame", function (player1Stack1, player1Stack2, player1Stack3, player1Stack4, player2Stack1, player2Stack2, player2Stack3, player2Stack4) {
    $("#player1Stack1").attr("src", "/images/" + player1Stack1 + ".svg");
    $("#player1Stack2").attr("src", "/images/" + player1Stack2 + ".svg");
    $("#player1Stack3").attr("src", "/images/" + player1Stack3 + ".svg");
    $("#player1Stack4").attr("src", "/images/" + player1Stack4 + ".svg");
    $("#player2Stack1").attr("src", "/images/" + player2Stack1 + ".svg");
    $("#player2Stack2").attr("src", "/images/" + player2Stack2 + ".svg");
    $("#player2Stack3").attr("src", "/images/" + player2Stack3 + ".svg");
    $("#player2Stack4").attr("src", "/images/" + player2Stack4 + ".svg");
});

connection.on("test", function () {
    $("#test").html("<br/><span><strong>Something went wrong.</strong></span>");
});

connection.on('registrationComplete', data => {
    $("#register").hide();
    $("#findOpponent").show();
});

connection.on('opponentFound', data => {
    $('#findOpponent').hide();
    $('#findingOpponent').hide();
    $('#game').show();
    $('#test').html("<br/><span><strong> Hey " + playerName + "! You are playing against <i>" + data + "</i></strong></span>");
    opponentName = data;
});

connection.on('opponentNotFound', data => {
    $('#findOpponent').hide();
    $('#findingOpponent').show();
});

connection.on('resetCounter', function (wantsReset) {
    if (wantsReset) {
        $('#resetButton').html("Your Opponent Wants To Reset.")
        opponentReset = true;
    }
    else if (playerReset) {
        $('#resetButton').html("You Want To Reset.")
        opponentReset = false;
    }
    else {
        $('#resetButton').html("No One Wants To Reset.")
        opponentReset = false;
    }
});

connection.on('resetCounterToZero', function () {
    playerReset = false;
    opponentReset = false;
    $('#resetButton').html("No One Wants To Reset.");
});

connection.on('cardCount', function (cardAmount) {
    $('#cardCount').html(cardAmount + " Cards Left.");
});



connection.on('opponentDisconnected', data => {
    $("#register").hide();
    $('#game').hide();
    $('#test').html("<br/><span><strong>" + playerName + ", your opponent has disconnected.</strong></span>");

});

connection.on('gameOver', function (shobu) {
    $("#game").hide();
    if (shobu == "won") {
        $('#test').html("<br/><span><strong>" + playerName + ", you've gotten rid of all your cards. You win!</strong></span> <br/><span><strong>Waiting to see if your opponent wants a rematch...</strong></span>");
    }
    else if (shobu == "lost") {
        $('#newGame').show();
        $('#test').html("<br/><span><strong>Sorry " + playerName + ", your opponent has gotten rid of all their cards. You lose!</strong></span> <br/><span><strong>Press \"New Game\" to play a new game against your current opponent.</strong></span>");
    }
});

connection.on('newGame', function () {
    $('#newGame').hide();
    $('#game').show();
    $('#test').html("<br/><span><strong> Hey " + playerName + "! You are playing against <i>" + opponentName + "</i></strong></span>");
});

$("#btnRegister").click(function () {
    playerName = $('#name').val();
    connection.invoke('RegisterPlayer', playerName);
});

$("#btnFindOpponentPlayer").click(function () {
    connection.invoke('FindOpponent');
});

$("#btnNewGame").click(function () {
    connection.invoke('NewGame');
});

function allowDrop(ev) {
    ev.preventDefault();
}

function drag(ev) {
    
}

function resetGame() {
    if (!playerReset) {
        playerReset = true;
        $('#resetButton').html("You Want To Reset.");
        connection.invoke("ResetGame", playerReset).catch(function (err) {
            return console.error(err.toString());
        });
    }
    else {
        playerReset = false;
        if (opponentReset)
            $('#resetButton').html("Your Opponent Wants To Reset.");
        else
            $('#resetButton').html("No One Wants To Reset.");
        connection.invoke("ResetGame", playerReset).catch(function (err) {
            return console.error(err.toString());
        });
    }
}

function drop(ev) {
    ev.preventDefault();
    connection.invoke("SendCard", ev.target.id).catch(function (err) {
        return console.error(err.toString());
    });
}

connection.start().catch(function (err) {
    return console.error(err.toString());
});