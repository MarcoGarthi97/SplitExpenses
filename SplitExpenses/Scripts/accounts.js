$(document).ready(function () {
    var accounts = []
    var users = []

    var idIncremental

    var chat = $.connection.chatHub;

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

    function GetBalance(){
        $.ajax({
            url: urlGetBalance,
            type: "POST",
            success: function (result) {
                if (result != "") {
                    CreateBalance(result)
                }
            },
            error: function (error) {
                console.log("error")
                console.log(error)
            }
        })
    }

    function CreateBalance(balance){
        $("#divBalance").remove()

        var positive = 0
        var negative = 0

        balance.forEach(function(key){
            if(key.Balance > 0)
                positive += key.Balance
            else
                negative += key.Balance
        })

        var list = []
        var perc

        balance.forEach(function(key){
            if(key.Balance > 0)
                perc = key.Balance * 100 / positive
            else
                perc = key.Balance * 100 / negative

            key.Perc = perc.toFixed(2)

            list.push(key)
        })

        list.sort((a, b) => b.Balance - a.Balance)
        
        var row = ""
        var classCss = ""
        list.forEach(function(key){
            if(key.Balance > 0)
                classCss = 'colored-bar-green'
            else
                classCss = 'colored-bar-red'

            var b = key.Balance.toFixed(2)

            row += '<div class="row"><div class="col-1"><p>' + key.Name + '</p></div><div class="col-9"><div class="colored-bar ' + classCss +'" style="width: ' + key.Perc +'%;"></div></div><div class="col-2"><p>Balance: ' + b +'</p></div></div>'
        })

        row = '<div id="divBalance">' + row + '<div>'
        $('#footer').append(row)
    }

    $(document).on('keyup', '#txtFilter', function () {
        var val = $('#txtFilter').val()
        if (val != "") {
            $.ajax({
                url: urlGetUsers,
                data: { filter: val },
                type: "POST",
                success: function (result) {
                    if (result != "") {
                        CreateSelect(result)
                    }
                },
                error: function (error) {
                    console.log("error")
                    console.log(error)
                }
            })
        }
    })

    function CreateSelect(users) {
        $('#selectSearch').empty();
        var option = ""
        users.forEach(function (key) {
            option += '<option value"' + key + '">' + key + '</option>'
        })

        $('#selectSearch').append(option)
    }

    $(document).click(function (e) {
        if (e.target.id == "btnAddUser") {
            var val = $('#selectSearch').val()
            if (val != "" && !users.find(x => x == val)) {
                users.push(val)

                var html = '<div class="col-6" id="divUser_' + val + '"><div class="row"><div class="col-10"><p>' + val + '</p></div><div class="col-2"><input type="button" class="btn btn-outline-danger btnDeleteUsers" id="btnDeleteUser_' + val + '" value="x"></div></div></div>'
                $('#divUsers').append(html)
            }
        }
    })

    $(document).on('click', '#btnAddAccount', function (e) {
        var name = $('#txtName').val()
        console.log(users)
        if (name != "") {
            $.ajax({
                url: urlInsertAccount,
                data: { name: name, users: users },
                type: "POST",
                success: function (result) {
                    if (result != "") {
                        accounts = result
                        CreateTable(accounts)

                        $('#btnCloseModal').click()

                        $('#txtName').value = ""
                        users = []
                    }
                },
                error: function (error) {
                    console.log("error")
                    console.log(error)
                }
            })
        }
        else
            alert('Insert the name of new account')
    })

    $(document).on('click', '#btnDeleteAccount', function () {
        console.log(idIncremental)
        $.ajax({
            url: urlDeleteAccount,
            data: { idIncremental: idIncremental },
            type: "POST",
            success: function (result) {
                if (result) {
                    $('#modalDeleteAccount').modal('hide')

                    GetAccounts()
                }
            },
            error: function (error) {
                console.log("error")
                console.log(error)
            }
        })
    })
    
    function GetExpenses() {
        $.ajax({
            url: urlGetExpense,
            type: "POST",
            success: function (result) {
                if (result != "") {
                    console.log(result)
                    CreateTableExpenses(result)
                }
            },
            error: function (error) {
                console.log("error")
                console.log(error)
            }
        })
    }

    function CreateTableExpenses(expenses) {
        const element = document.getElementById('tbody')
        if (element != null)
            element.remove()

        var rows = ""
        expenses.forEach(function (item) {
            var d = item.Date.substring(7, 19)
            var date = new Date(d)
            console.log(item)
            console.log(item.Date)
            console.log(d)
            console.log(date)
            rows += "<tr><td>" + item.Name + "</td><td>" + item.PaidBy + "</td><td>" + date + "</td><td>" + item.Cost
                + '</td><td><div class="row"><div class="col"><input type="button" class="btn btn-outline-danger btnDeleteExpenses" id="btnDelete_' + item.Id.Increment + '" value="Delete"/></div>'
                + '<div class="col"><input type="button" class="btn btn-outline-primary btnInfoExpenses" id="btnInfo_' + item.Id.Increment + '" value="Info"/></div></tr>'
        })

        $('#tableExpenses').append('<tbody id="tbody">' + rows + '</tbody>')
    }

    document.addEventListener('click', function (e) {
        if (e.target.className.split(' ')[2] == "btnAccounts") {
            idIncremental = e.target.id.substring(4)
            $.ajax({
                url: urlAccount,
                data: { idIncremental: idIncremental },
                type: "POST",
                success: function (result) {
                    if (result != "") {
                        $('#divLoadComponent').empty()

                        $('#divLoadComponent').html(result)

                        GetExpenses()
                    }
                },
                error: function (error) {
                    console.log("error")
                    console.log(error)
                }
            })
        }
        else if (e.target.className.split(' ')[2] == "btnDeleteAccounts") {
            idIncremental = e.target.id.substring(10)

            $('#modalDeleteAccount').modal('show')
        }
        else if (e.target.className.split(' ').find(x => x == "btnDeleteUsers")) {
            var username = e.target.id.substring(14)
            console.log(username)
            $('#divUser_' + username).remove()

            var index = users.indexOf(username)
            users.splice(index, 1)
        }
    })

    $(function () {
        chat.client.getExpenses = function () {
            GetExpenses()
        };

        chat.client.getBalance = function () {
            GetBalance()
        };
    });
})