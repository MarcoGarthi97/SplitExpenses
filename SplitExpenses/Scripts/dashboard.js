$(document).ready(function () {
    const btnDashboard = document.getElementById('btnDashboard')

    const spanCount = document.getElementById('spanCount')

    var _user = ''
    var chat = $.connection.chatHub;

    btnDashboard.addEventListener('click', function () {
        LoadComponents()
    })

    $('#btnNotification').on('click', function () {
        $('#modalInviteAccount').modal('show')
        Getinvites()
    })

    $('#btnLogout').on('click', function () {
        $.ajax({
            url: urlLogout,
            type: "POST",
            success: function () {
                window.location.href = urlIndex
            },
            error: function (error) {
                console.log("error")
                console.log(error)
            }
        })
    })

    function Getinvites() {
        chat.server.notify(_user)

        $.ajax({
            url: urlGetInvites,
            type: "POST",
            success: function (result) {
                $('#tbodyInvites').remove()
                if (result != "")
                    CreateModalComponents(result)
            },
            error: function (error) {
                console.log("error")
                console.log(error)
            }
        })
    }

    function CreateModalComponents(invites) {
        var row = ''
        invites.forEach(function (key) {
            var owner = key.Users.find(x => x.Owner == true).Name

            row += '<tr><td>' + key.Name + '</td><td>' + owner + '</td><td><button type="button" class="btn btn-danger btnDeleteInvites" id="btnDeleteInvite_' + key.Id.Increment
                + '">X</button></td><td><button class="btn btn-primary btnAcceptInvites" id="btnAcceptInvite_' + key.Id.Increment + '">></button></td></tr>'
        })

        row = '<tbody id="tbodyInvites">' + row + '</tbody>'
        $('#tableInvites').append(row)
    }

    $(document).on('click', '.btnAcceptInvites', function (e) {
        var idIncremental = e.target.id.substring(16)

        $.ajax({
            url: urlUpdateInvites,
            type: "POST",
            data: { idIncremental: idIncremental, val: 1 },
            success: function (result) {
                if (result)
                    Getinvites()
            },
            error: function (error) {
                console.log("error")
                console.log(error)
            }
        })
    })

    $(document).on('click', '.btnDeleteInvites', function (e) {
        var idIncremental = e.target.id.substring(16)

        $.ajax({
            url: urlUpdateInvites,
            type: "POST",
            data: { idIncremental: idIncremental, val: 0 },
            success: function (result) {
                if (result)
                    Getinvites()
            },
            error: function (error) {
                console.log("error")
                console.log(error)
            }
        })
    })

    $('.closeModalInvites').on('click', function () {
        GetAccounts()
    })

    function LoadComponents() {
        $('#divLoadComponent').empty()

        $.ajax({
            url: urlAccounts,
            type: "POST",
            success: function (result) {
                $('#divLoadComponent').html(result)
                GetAccounts()
            },
            error: function (error) {
                console.log("error")
                console.log(error)
            }
        })
    }

    function GetAccounts() {
        $.ajax({
            url: urlGetAccounts,
            type: "POST",
            success: function (result) {
                if (result != "") {
                    accounts = result
                    CreateTable(accounts)
                }
            },
            error: function (error) {
                console.log("error")
                console.log(error)
            }
        })
    }

    function CreateTable(accounts) {
        const element = document.getElementById('tbody')
        if (element != null)
            element.remove()

        var rows = ""
        accounts.forEach(function (item) {
            var users = ""
            item.Users.forEach(function (key) {
                if (JSON.stringify(item.Users.slice(-1)) === JSON.stringify(Object.is(key)))
                    users += key.Name
                else
                    users += key.Name + " - "
            })
            rows += "<tr><td>" + item.Name + "</td><td>" + item.UserExpenses.toFixed(2) + "</td><td>" + item.TotalExpenses.toFixed(2) + "</td><td>" + users
                + '</td><td><input type="button" class="btn btn-outline-danger btnDeleteAccounts" id="btnDelete_' + item.Id.Increment + '" value="X"/>'
                + '</td><td><input type="button" class="btn btn-outline-primary btnInfoAccounts" id="btnInfo_' + item.Id.Increment + '" value="?"/>'
                + '</td><td><input type="button" class="btn btn-outline-primary btnAccounts" id="btn_' + item.Id.Increment + '" value=">"/></td></tr>'
        })

        $('#tableAccounts').append('<tbody id="tbody">' + rows + '</tbody>')
    }

    GetInfoUser()
    function GetInfoUser() {
        $.ajax({
            url: urlGetInfoUser,
            type: "POST",
            success: function (result) {
                _user = result

                $('#btnDashboard').click()

                $.connection.hub.start().done(function () {
                    chat.server.notify(_user)
                });

                LoadComponents()
            },
            error: function (error) {
                console.log("error")
                console.log(error)
            }
        })
    }

    $(function () {
        chat.client.GetCountInvitations = function (result) {
            spanCount.innerHTML = result
        };
    });
})