$(document).ready(function () {
    const btnDashboard = document.getElementById('btnDashboard')

    const spanCount = document.getElementById('spanCount')

    var _user = ''
    var chat = $.connection.chatHub;

    //$("#btnDashboard").trigger("click");

    btnDashboard.addEventListener('click', function () {
        LoadComponents()
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
        //var url = '@Url.Action("Accounts", "Home")'
        //$('#divLoadComponent').html(url)
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
                + '</td><td><input type="button" class="btn btn-outline-primary btnInfoAccounts" id="btnInfo_' + item.Id.Increment + '" value="!"/>'
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